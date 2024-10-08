using Guppy.Core.Common.Attributes;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Graphics.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [GraphicsEnabled]
    internal class TractorBeamHighlightEngine(
        ICamera2D camera,
        IEntityQueryService entityQueryService,
        ISocketService socketService,
        ITractorBeamEmitterService tractorBeamEmitterService,
        IUserShipService userShipService,
        ILogger logger) : StrategyEngine, IOnDrawEngine
    {
        private readonly ILogger _logger = logger;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISocketService _socketService = socketService;
        private readonly ICamera2D _camera = camera;
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;
        private readonly IUserShipService _userShipService = userShipService;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public override void Ready()
        {
            base.Ready();
        }

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PostDraw)]
        public void OnDraw(GameTime gameTime)
        {
            // if (_userShips.TryGetCurrentUserShipId(out EntityId shipId) == false)
            // {
            //     return;
            // }
            // 
            // if (_tractorBeamEmitters.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
            // {
            //     return;
            // }
            // 
            // _visibleRenderingService.Begin(_resources.Get(Resources.Colors.TractorBeamHighlight), Color.Red);
            // try
            // {
            //     this.FillVisibleRecursive(targetNode.Id);
            // }
            // catch (Exception e)
            // {
            //     _logger.Warning(e, "{ClassName}::{MethodName} - Exception while attempting to render {TargetNodeVhId}. This may be caused by frame step desync and should self correct.", nameof(TractorBeamHighlightEngine), nameof(Step), targetNode.Id.VhId.Value);
            // }
            // _visibleRenderingService.End();
        }

        private void FillVisibleRecursive(EntityId id)
        {
            return;

            // ref EntityStatus status = ref _entities.QueryById<EntityStatus>(id, out GroupIndex groupIndex);
            // 
            // if (!status.IsSpawned)
            // {
            //     return;
            // }
            // 
            // ref Node node = ref _entities.QueryByGroupIndex<Node>(in groupIndex);
            // ref Visible visible = ref _entities.QueryByGroupIndex<Visible>(in groupIndex);
            // 
            // Matrix transformation = node.XnaTransformation;
            // _visibleRenderingService.Draw(in visible, ref transformation);
            // 
            // if (_entities.TryQueryByGroupIndex<Sockets<SocketId>>(in groupIndex, out Sockets<SocketId> sockets))
            // {
            //     for (int i = 0; i < sockets.Items.count; i++)
            //     {
            //         var filter = _sockets.GetCouplingFilter(sockets.Items[i]);
            //         foreach (var (indices, groupId) in filter)
            //         {
            //             var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);
            // 
            //             for (int j = 0; j < indices.count; j++)
            //             {
            //                 this.FillVisibleRecursive(entityIds[indices[j]]);
            //             }
            //         }
            //     }
            // }
        }
    }
}
