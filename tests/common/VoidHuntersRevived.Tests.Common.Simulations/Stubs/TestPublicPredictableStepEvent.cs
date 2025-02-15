using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations.Stubs
{
    public class TestPublicPredictableStepEvent : IStepEvent<TestPublicPredictableStepEvent>
    {
        public bool IsPrivate => false;
        public bool IsPredictable => true;

        public VhId CalculateHash(in VhId sourceId)
        {
            return HashBuilder<TestPublicPredictableStepEvent, VhId>.Instance.Calculate(sourceId);
        }
    }
}
