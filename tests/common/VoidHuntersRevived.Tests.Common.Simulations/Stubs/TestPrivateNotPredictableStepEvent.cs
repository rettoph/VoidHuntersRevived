using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations.Stubs
{
    public class TestPrivateNotPredictableStepEvent : IStepEvent<TestPrivateNotPredictableStepEvent>
    {
        public bool IsPrivate => false;
        public bool IsPredictable => false;

        public VhId CalculateHash(in VhId sourceId)
        {
            return HashBuilder<TestPrivateNotPredictableStepEvent, VhId>.Instance.Calculate(sourceId);
        }
    }
}
