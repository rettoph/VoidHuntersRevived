using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public sealed class EntitySubmissionSystem(EntitiesSubmissionScheduler scheduler) : ISceneSystem, IEngine, IStepSystem
    {
        private readonly EntitiesSubmissionScheduler _scheduler = scheduler;

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.SubmitChanges)]
        public void Step(Step step)
        {
            this._scheduler.SubmitEntities();
        }
    }
}