using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Client.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    [SimulationFilter(SimulationType.Predictive)]
    internal class TractorBeamHighlightEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly IResourceProvider _resources;
        private readonly ISocketService _sockets;
        private readonly Camera2D _camera;
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;
        private readonly IUserShipService _userShips;

        public string name { get; } = nameof(TractorBeamHighlightEngine);

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public TractorBeamHighlightEngine(
            ILogger logger,
            IEntityService entities,
            IVisibleRenderingService visibleRenderingService,
            IResourceProvider resources,
            ISocketService sockets,
            Camera2D camera,
            ITractorBeamEmitterService tractorBeamEmitters,
            IUserShipService userShips)
        {
            _camera = camera;
            _entities = entities;
            _visibleRenderingService = visibleRenderingService;
            _resources = resources;
            _sockets = sockets;
            _logger = logger;
            _tractorBeamEmitters = tractorBeamEmitters;
            _userShips = userShips;
        }

        public override void Ready()
        {
            base.Ready();
        }

        public void Step(in GameTime _param)
        {
            if (_userShips.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            if (_tractorBeamEmitters.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
            {
                return;
            }

            _visibleRenderingService.Begin(_resources.Get(Resources.Colors.TractorBeamHighlight), Color.Red);
            try
            {
                this.FillVisibleRecursive(targetNode.Id);
            }
            catch (Exception e)
            {
                _logger.Warning(e, "{ClassName}::{MethodName} - Exception while attempting to render {TargetNodeVhId}. This may be caused by frame step desync and should self correct.", nameof(TractorBeamHighlightEngine), nameof(Step), targetNode.Id.VhId.Value);
            }
            _visibleRenderingService.End();
        }

        private void FillVisibleRecursive(EntityId id)
        {
            return;

            ref EntityStatus status = ref _entities.QueryById<EntityStatus>(id, out GroupIndex groupIndex);

            if (!status.IsSpawned)
            {
                return;
            }

            ref Node node = ref _entities.QueryByGroupIndex<Node>(in groupIndex);
            ref Visible visible = ref _entities.QueryByGroupIndex<Visible>(in groupIndex);

            Matrix transformation = node.XnaTransformation;
            _visibleRenderingService.Draw(in visible, ref transformation);

            if (_entities.TryQueryByGroupIndex<Sockets<SocketId>>(in groupIndex, out Sockets<SocketId> sockets))
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
