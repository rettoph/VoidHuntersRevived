using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    [Sequence<StepSequence>(StepSequence.EntitySubmission)]
    public sealed class EntitySubmissionEngine : IEngine, IStepEngine<Step>
    {
        private readonly EntitiesSubmissionScheduler _scheduler;

        public EntitySubmissionEngine(EntitiesSubmissionScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public string name { get; } = nameof(EntitySubmissionEngine);

        public void Step(in Step _param)
        {
            _scheduler.SubmitEntities();
        }
    }
}
