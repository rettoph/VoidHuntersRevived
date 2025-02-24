using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Entities.Stubs
{
    public class TestDepawnEntityStepInput : IStepInput<TestDepawnEntityStepInput>
    {
        public bool IsPredictable => true;

        public required EntityGlobalId EntityGlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestDepawnEntityStepInput, VhId, EntityGlobalId>.Instance.Calculate(source, this.EntityGlobalId);
        }

        public static TestDepawnEntityStepInput Factory(int id)
        {
            return new() { EntityGlobalId = HashBuilder<TestDepawnEntityStepInput, int>.Instance.Calculate(id).ToGlobalEntityId() };
        }
    }
}