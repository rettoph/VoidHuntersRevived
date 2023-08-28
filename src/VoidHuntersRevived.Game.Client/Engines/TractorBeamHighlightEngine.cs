using Guppy.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network.Peers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Network.Identity;
using VoidHuntersRevived.Common.Entities.Services;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces;
using Guppy.Common.Attributes;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Pieces.Services;
using static VoidHuntersRevived.Common.Resources;
using VoidHuntersRevived.Game.Ships.Services;
using System.Net.Sockets;
using VoidHuntersRevived.Common.Ships.Services;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal class TractorBeamHighlightEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly IResourceProvider _resources;
        private readonly ISocketService _sockets;
        private readonly Camera2D _camera;
        private readonly ClientPeer _client;
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;

        public string name { get; } = nameof(TractorBeamHighlightEngine);

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public TractorBeamHighlightEngine(
            ILogger logger, 
            IEntityService entities,
            IVisibleRenderingService visibleRenderingService,
            IResourceProvider resources,
            ISocketService sockets,
            Camera2D camera, 
            ClientPeer client,
            ITractorBeamEmitterService tractorBeamEmitters)
        {
            _camera = camera;
            _entities = entities;
            _visibleRenderingService = visibleRenderingService;
            _resources = resources;
            _sockets = sockets;
            _logger = logger;
            _client = client;
            _tractorBeamEmitters = tractorBeamEmitters;
        }

        public override void Ready()
        {
            base.Ready();
        }

        public void Step(in GameTime _param)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            if (!_entities.TryGetId(_client.Users.Current.GetUserShipId(), out EntityId shipId))
            {
                return;
            }

            if (!_tractorBeamEmitters.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode))
            {
                return;
            }

            _visibleRenderingService.BeginFill();
            try
            {
                this.FillVisibleRecursive(targetNode.Id);
            }
            catch(Exception e)
            {
                _logger.Warning(e, "{ClassName}::{MethodName} - Exception while attempting to render {TargetNodeVhId}. This may be caused by frame step desync and should self correct.", nameof(TractorBeamHighlightEngine), nameof(Step), targetNode.Id.VhId.Value);
            }
            _visibleRenderingService.End();
        }

        private void FillVisibleRecursive(EntityId id)
        {
            ref Node node = ref _entities.QueryById<Node>(id, out GroupIndex groupIndex);
            ref Visible visible = ref _entities.QueryByGroupIndex<Visible>(in groupIndex);

            Matrix transformation = node.Transformation.XnaMatrix;
            _visibleRenderingService.Fill(in visible, ref transformation, _resources.Get(Colors.TractorBeamHighlight));

            if(_entities.TryQueryByGroupIndex<Sockets>(in groupIndex, out Sockets sockets))
            {
                for (int i = 0; i < sockets.Items.count; i++)
                {
                    var filter = _sockets.GetCouplingFilter(sockets.Items[i].Id);
                    foreach (var (indices, groupId) in filter)
                    {
                        var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);

                        for (int j = 0; j < indices.count; j++)
                        {
                            this.FillVisibleRecursive(entityIds[indices[j]]);
                        }
                    }
                }
            }
        }
    }
}
