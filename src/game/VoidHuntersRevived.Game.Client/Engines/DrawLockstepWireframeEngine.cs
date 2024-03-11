using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Messaging;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Client.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Messages;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Lockstep)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal sealed class DrawLockstepWireframeEngine : BasicEngine, IStepEngine<GameTimeTeam>,
        ISubscriber<Input_Toggle_LockstepWireframe>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup[]> _teamDescriptorGroups;
        private bool _visible;

        public string name { get; } = nameof(DrawLockstepWireframeEngine);

        public DrawLockstepWireframeEngine(
            ILogger logger,
            IVisibleRenderingService visibleRenderingService,
            IEntityService entities,
            ITeamDescriptorGroupService teamDescriptorGroups)
        {
            _visibleRenderingService = visibleRenderingService;
            _entities = entities;
            _indexBuffer = new short[3];
            _logger = logger;

            _teamDescriptorGroups = teamDescriptorGroups.GetAllWithComponentsByTeams(typeof(Visible), typeof(Node));
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTimeTeam _param)
        {
            if (!_visible)
            {
                return;
            }

            //_visibleRenderingService.BeginTrace();
            //foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            //{
            //    var (ids, statuses, visibles, nodes, count) = _entities.QueryEntities<EntityId, EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);
            //    for (int index = 0; index < count; index++)
            //    {
            //        try
            //        {
            //            if (statuses[index].IsSpawned)
            //            {
            //                Matrix transformation = nodes[index].Transformation.ToTransformationXnaMatrix();
            //                _visibleRenderingService.Trace(in visibles[index], ref transformation, Color.Red);
            //                // _visibleRenderingService.Trace(in visibles[index], ref transformation, this.Simulation.Type == SimulationType.Predictive ? Color.Yellow : Color.Red);
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(DrawLockstepWireframeEngine), nameof(Step), ids[index].VhId.Value);
            //        }
            //    }
            //}
            //_visibleRenderingService.EndTrace();
        }

        public void Process(in Guid messageId, Input_Toggle_LockstepWireframe message)
        {
            _visible = !_visible;
        }
    }
}
