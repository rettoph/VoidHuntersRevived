using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations.Stubs
{
    public class TestPublicPredictableStepInput : IStepInput<TestPublicPredictableStepInput>
    {
        public bool IsPrivate => false;
        public bool IsPredictable => true;

        public VhId CalculateHash(in VhId sourceId)
        {
            return HashBuilder<TestPublicPredictableStepInput, VhId>.Instance.Calculate(sourceId);
        }
    }
}
