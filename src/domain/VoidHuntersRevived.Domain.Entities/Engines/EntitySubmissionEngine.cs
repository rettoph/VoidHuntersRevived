using Guppy.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    [Sequence<StepSequence>(StepSequence.EntitySubmission)]
    internal sealed class EntitySubmissionEngine : IEngine, IStepEngine<Step>
    {
        private readonly IEntityService _entities;

        public EntitySubmissionEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public string name { get; } = nameof(EntitySubmissionEngine);

        public void Step(in Step _param)
        {
            _entities.Flush();
        }
    }
}
