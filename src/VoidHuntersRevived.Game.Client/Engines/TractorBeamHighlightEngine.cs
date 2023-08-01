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
using VoidHuntersRevived.Game.Services;
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
        private readonly Camera2D _camera;
        private readonly ClientPeer _client;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public string name { get; } = nameof(TractorBeamHighlightEngine);

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public TractorBeamHighlightEngine(
            ILogger logger, 
            IEntityService entities,
            IVisibleRenderingService visibleRenderingService,
            IResourceProvider resources,
            Camera2D camera, 
            ClientPeer client,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _camera = camera;
            _entities = entities;
            _visibleRenderingService = visibleRenderingService;
            _resources = resources;
            _logger = logger;
            _client = client;
            _tractorBeamEmitterService = tractorBeamEmitterService;
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

            if (!_tractorBeamEmitterService.Query(shipId, (FixVector2)this.CurrentTargetPosition, out EntityId targetId))
            {
                return;
            }

            _visibleRenderingService.BeginFill();

            this.FillVisibleRecursive(targetId.EGID);

            _visibleRenderingService.End();
        }

        private void FillVisibleRecursive(EGID egid)
        {
            var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(egid, out uint index);
            var (visibles, _) = this.entitiesDB.QueryEntities<Visible>(egid.groupID);

            Matrix transformation = nodes[index].Transformation.XnaMatrix;
            _visibleRenderingService.Fill(in visibles[index], ref transformation, _resources.Get(Colors.TractorBeamHighlight));

            if (this.entitiesDB.TryGetEntityByIndex<Sockets>(index, egid.groupID, out Sockets sockets))
            {
                for(int i=0; i<sockets.Items.count; i++)
                {
                    if(sockets.Items[i].PlugId.VhId != default)
                    {
                        this.FillVisibleRecursive(sockets.Items[i].PlugId.EGID);
                    }
                }
            }
        }
    }
}
