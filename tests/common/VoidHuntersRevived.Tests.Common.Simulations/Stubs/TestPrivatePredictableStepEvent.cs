using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations.Stubs
{
    public class TestPrivatePredictableStepEvent : IStepEvent<TestPrivatePredictableStepEvent>
    {
        public bool IsPrivate => false;
        public bool IsPredictable => true;

        public VhId CalculateHash(in VhId sourceId)
        {
            return HashBuilder<TestPrivatePredictableStepEvent, VhId>.Instance.Calculate(sourceId);
        }
    }
}
