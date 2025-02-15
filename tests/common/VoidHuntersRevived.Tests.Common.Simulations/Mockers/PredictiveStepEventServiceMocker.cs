using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
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

        public static PredictiveStepEventServiceMocker Create()
        {
            return new PredictiveStepEventServiceMocker();
        }
    }
}
