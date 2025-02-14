using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Stubs;
using VoidHuntersRevived.Tests.Domain.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Domain.Simulations
{
    public class PredictiveEventServiceTests
    {
        [Fact]
        public void EnqueuePrivateNotPredictableStepEventThenFlush_IsNotPublished()
        {
            PredictiveStepEventServiceMock.Create().EnqueueFlushAndVerifyPublish<TestPrivateNotPredictableStepEvent>(
                sourceId: VhId.NewVhId(),
                publishTimes: Times.Never);
        }

        [Fact]
        public void EnqueuePrivatePredictableStepEventThenFlush_IsPublished()
        {
            PredictiveStepEventServiceMock.Create().EnqueueFlushAndVerifyPublish<TestPrivatePredictableStepEvent>(
                sourceId: VhId.NewVhId(),
                publishTimes: Times.Once);
        }
    }
}
