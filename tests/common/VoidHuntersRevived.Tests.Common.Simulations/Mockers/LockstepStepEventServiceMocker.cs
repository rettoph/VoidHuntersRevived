using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class LockstepStepEventServiceMocker
    {
        public Mocker<IMessageBus> MessageBusMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }
        public readonly LockstepStepEventService LockstepEventService;

        public LockstepStepEventServiceMocker()
        {
            this.MessageBusMocker = new Mocker<IMessageBus>();
            this.LoggerMocker = new Mocker<ILogger>();
            this.LockstepEventService = new LockstepStepEventService(
                this.MessageBusMocker.GetInstance(),
                this.LoggerMocker.GetInstance());
        }

        public static PredictiveStepEventServiceMocker Create()
        {
            return new PredictiveStepEventServiceMocker();
        }
    }
}
