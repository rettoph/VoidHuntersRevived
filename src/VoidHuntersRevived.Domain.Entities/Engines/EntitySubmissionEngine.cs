using Guppy.Common.Attributes;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [Sequence<StepSequence>(StepSequence.OnEntitySubmit)]
    internal sealed class EntitySubmissionEngine : IEngine, IStepEngine<Step>
    {
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;

        public EntitySubmissionEngine(SimpleEntitiesSubmissionScheduler scheduler)
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
