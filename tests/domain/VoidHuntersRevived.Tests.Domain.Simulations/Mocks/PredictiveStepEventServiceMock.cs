using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Mocks
{
    public class PredictiveStepEventServiceMock
    {
        public readonly Mock<IMessageBus> MessageBus;
        public readonly Mock<ILogger> Logger;
        public readonly PredictiveStepEventService PredictiveEventService;

        public PredictiveStepEventServiceMock()
        {
            this.MessageBus = new Mock<IMessageBus>();
            this.Logger = new Mock<ILogger>();
            this.PredictiveEventService = new PredictiveStepEventService(
                this.MessageBus.Object,
                this.Logger.Object);
        }

        public void EnqueueFlushAndVerifyPublish<TEvent>(VhId sourceId, Func<Times> publishTimes)
            where TEvent : IStepEvent, new()
        {
            // Publish event
            this.PredictiveEventService.Enqueue(sourceId, new TEvent());

            // Flush service
            this.PredictiveEventService.Flush();

            // Verify publish
            this.MessageBus.Verify(
                x => x.Publish<EventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                publishTimes);
        }

        public static PredictiveStepEventServiceMock Create()
        {
            return new PredictiveStepEventServiceMock();
        }
    }
}
