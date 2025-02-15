using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Extensions
{
    public static class PredictiveStepEventServiceMockerExtensions
    {
        public static void EnqueueFlushAndVerifyPublish<TEvent>(
            this PredictiveStepEventServiceMocker predictiveStepEventServiceMocker,
            VhId sourceId,
            Func<Times> publishTimes
        )
            where TEvent : IStepEvent, new()
        {
            // Publish event
            predictiveStepEventServiceMocker.PredictiveEventService.Enqueue(sourceId, new TEvent());

            // Flush service
            predictiveStepEventServiceMocker.PredictiveEventService.Flush();

            // Verify publish
            predictiveStepEventServiceMocker.MessageBusMocker.Verify(
                x => x.Publish<EventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                publishTimes);
        }
    }
}
