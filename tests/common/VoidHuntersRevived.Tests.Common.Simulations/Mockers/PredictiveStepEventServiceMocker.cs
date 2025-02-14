using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class PredictiveStepEventServiceMocker
    {
        public Mocker<IMessageBus> MessageBusMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }
        public readonly PredictiveStepEventService PredictiveEventService;

        public PredictiveStepEventServiceMocker()
        {
            this.MessageBusMocker = new Mocker<IMessageBus>();
            this.LoggerMocker = new Mocker<ILogger>();
            this.PredictiveEventService = new PredictiveStepEventService(
                this.MessageBusMocker.GetInstance(),
                this.LoggerMocker.GetInstance());
        }

        public void EnqueueFlushAndVerifyPublish<TEvent>(VhId sourceId, Func<Times> publishTimes)
            where TEvent : IStepEvent, new()
        {
            // Publish event
            this.PredictiveEventService.Enqueue(sourceId, new TEvent());

            // Flush service
            this.PredictiveEventService.Flush();

            // Verify publish
            this.MessageBusMocker.Verify(
                x => x.Publish<EventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                publishTimes);
        }

        public static PredictiveStepEventServiceMocker Create()
        {
            return new PredictiveStepEventServiceMocker();
        }
    }
}
