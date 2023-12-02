using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network.Identity;
using Guppy.Network.Peers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Game.Common.Enums;
using static VoidHuntersRevived.Common.Resources;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
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

            _visibleRenderingService.Begin(_resources.Get(Colors.TractorBeamHighlight), Color.Red);
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
            ref EntityStatus status = ref _entities.QueryById<EntityStatus>(id, out GroupIndex groupIndex);

            if(!status.IsSpawned)
            {
                return;
            }

            ref Node node = ref _entities.QueryByGroupIndex<Node>(in groupIndex);
            ref Visible visible = ref _entities.QueryByGroupIndex<Visible>(in groupIndex);

            Matrix transformation = node.XnaTransformation;
            _visibleRenderingService.Draw(in visible, ref transformation);

            if(_entities.TryQueryByGroupIndex<Sockets<SocketId>>(in groupIndex, out Sockets<SocketId> sockets))
            {
                for (int i = 0; i < sockets.Items.count; i++)
                {
                    var filter = _sockets.GetCouplingFilter(sockets.Items[i]);
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
