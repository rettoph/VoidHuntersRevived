using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Entities.Stubs
{
    public class TestSpawnEntityStepInput : IStepInput<TestSpawnEntityStepInput>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId EntityGlobalId { get; init; }
        public required Key<IEntityTemplate> EntityTemplateKey { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestSpawnEntityStepInput, VhId, EntityGlobalId, VhId>.Instance.Calculate(source, this.EntityGlobalId, this.EntityTemplateKey.Id);
        }

        public static TestSpawnEntityStepInput Factory(int id)
        {
            return new() { EntityGlobalId = HashBuilder<TestSpawnEntityStepInput, int>.Instance.Calculate(id).ToGlobalEntityId(), EntityTemplateKey = TestEntityComponent.TestEntityTemplateKey };
        }
    }
}