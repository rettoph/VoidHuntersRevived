using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Extensions
{
    public static class PredictiveStepEventServiceBuilderExtensions
    {
        public static PredictiveStepEventServiceBuilder EnqueueFlushAndVerifyPublish<TEvent>(
            this PredictiveStepEventServiceBuilder predictiveStepEventServiceBuilder,
            VhId sourceId,
            Func<Times> publishTimes
        )
            where TEvent : IStepEvent, new()
        {
            // Publish event
            predictiveStepEventServiceBuilder.Object.Enqueue(sourceId, new TEvent());

            // Flush service
            predictiveStepEventServiceBuilder.Object.Flush();

            // Verify publish
            predictiveStepEventServiceBuilder.MessageBusMocker.Verify(
                x => x.Publish<EventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                publishTimes);

            return predictiveStepEventServiceBuilder;
        }
    }
}
