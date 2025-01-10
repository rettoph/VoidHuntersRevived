using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    public sealed class EntitySubmissionEngine(EntitiesSubmissionScheduler scheduler) : IEngine, IOnStepEngine
    {
        private readonly EntitiesSubmissionScheduler _scheduler = scheduler;

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SubmitChanges)]
        public void OnStep(Step step)
        {
            this._scheduler.SubmitEntities();
        }
    }
}