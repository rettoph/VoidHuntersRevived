using Guppy.Core.Common.Attributes;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    internal sealed class DrawActiveThrustableEngine(
        ILogger logger,
        IEntityQueryService entityQueryService,
        Camera2D camera) : StrategyEngine
    {
        private readonly short[] _indexBuffer = new short[3];
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;
        private readonly Camera2D _camera = camera;

        public string name { get; } = nameof(DrawActiveThrustableEngine);

        // public void Step(in GameTimeTeam _param)
        // {
        //     // Color activeThrustableHighlight = _resources.Get(Resources.Colors.ActiveThrustableHighlight);
        //     // _visibleRenderingService.Begin(activeThrustableHighlight, Color.Transparent);
        //     // foreach (var ((ids, helms, count), groupId) in _entities.QueryEntities<EntityId, Helm>())
        //     // {
        //     //     for (int i = 0; i < count; i++)
        //     //     {
        //     //         EntityId helmId = ids[i];
        //     //         Helm helm = helms[i];
        //     // 
        //     //         ref var helmThrustables = ref _entities.GetFilter<Thrustable>(helmId, Helm.ThrustableFilterContextId);
        //     // 
        //     //         this.TryDrawThrustableImpulse(_param, helm.Direction, ref helmThrustables);
        //     //     }
        //     // }
        //     // 
        //     // _visibleRenderingService.End();
        // }
        // 
        // private void TryDrawThrustableImpulse(GameTimeTeam param, Direction direction, ref EntityFilterCollection helmThrustables)
        // {
        //     // foreach (var (indices, group) in helmThrustables)
        //     // {
        //     //     var (pieceTypes, thrustables, nodes, _) = _entities.QueryEntities<Id<EntityContext>, Thrustable, Node>(group);
        //     // 
        //     //     for (int i = 0; i < indices.count; i++)
        //     //     {
        //     //         uint index = indices[i];
        //     //         ref Thrustable thrustable = ref thrustables[index];
        //     // 
        //     //         if ((thrustable.Direction & direction) == 0)
        //     //         {
        //     //             continue;
        //     //         }
        //     // 
        //     //         ref Node node = ref nodes[index];
        //     //         Matrix transformation = node.XnaTransformation;
        //     // 
        //     //         // _visibleRenderingService.Draw(in visibles[index], ref transformation);
        //     //     }
        //     // }
        // }
    }
}
