using Guppy.Core.Messaging.Common;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Game.Client.Messages;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    internal sealed class DrawLockstepWireframeEngine(
        ILogger logger,
        IEntityQueryService entityQueryService) : StrategyEngine,
        ISubscriber<Input_Toggle_LockstepWireframe>
    {
        private readonly short[] _indexBuffer = new short[3];
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;
        private bool _visible;

        public string name { get; } = nameof(DrawLockstepWireframeEngine);

        // public void Step(in GameTimeTeam _param)
        // {
        //     if (!_visible)
        //     {
        //         return;
        //     }
        // 
        //     //_visibleRenderingService.BeginTrace();
        //     //foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
        //     //{
        //     //    var (ids, statuses, visibles, nodes, count) = _entities.QueryEntities<EntityId, EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);
        //     //    for (int index = 0; index < count; index++)
        //     //    {
        //     //        try
        //     //        {
        //     //            if (statuses[index].IsSpawned)
        //     //            {
        //     //                Matrix transformation = nodes[index].Transformation.ToTransformationXnaMatrix();
        //     //                _visibleRenderingService.Trace(in visibles[index], ref transformation, Color.Red);
        //     //                // _visibleRenderingService.Trace(in visibles[index], ref transformation, this.Simulation.Type == SimulationType.Predictive ? Color.Yellow : Color.Red);
        //     //            }
        //     //        }
        //     //        catch (Exception e)
        //     //        {
        //     //            _logger.Error(e, "Exception attempting to fill shapes for visible {VisibleVhId}", ids[index].VhId.Value);
        //     //        }
        //     //    }
        //     //}
        //     //_visibleRenderingService.EndTrace();
        // }

        public void Process(in Guid messageId, Input_Toggle_LockstepWireframe message)
        {
            _visible = !_visible;
        }
    }
}
