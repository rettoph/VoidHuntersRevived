using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    public sealed class EntitySubmissionEngine : IEngine, IOnStepEngine
    {
        private readonly EntitiesSubmissionScheduler _scheduler;

        public EntitySubmissionEngine(EntitiesSubmissionScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SubmitChanges)]
        public void OnStep(Step step)
        {
            _scheduler.SubmitEntities();
        }
    }
}
