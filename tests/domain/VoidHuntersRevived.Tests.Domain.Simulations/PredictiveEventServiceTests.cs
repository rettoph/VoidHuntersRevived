using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using VoidHuntersRevived.Tests.Common.Simulations.Stubs;
using VoidHuntersRevived.Tests.Domain.Simulations.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Simulations
{
    public class PredictiveEventServiceTests
    {
        [Fact]
        public void EnqueuePrivateNotPredictableStepEventThenFlush_IsNotPublished()
        {
            PredictiveStepEventServiceMocker.Create().EnqueueFlushAndVerifyPublish<TestPrivateNotPredictableStepEvent>(
                sourceId: VhId.NewVhId(),
                publishTimes: Times.Never);
        }

        [Fact]
        public void EnqueuePrivatePredictableStepEventThenFlush_IsPublished()
        {
            PredictiveStepEventServiceMocker.Create().EnqueueFlushAndVerifyPublish<TestPrivatePredictableStepEvent>(
                sourceId: VhId.NewVhId(),
                publishTimes: Times.Once);
        }
    }
}
