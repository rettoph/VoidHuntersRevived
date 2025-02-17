using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class DefaultLockstepStepEventServiceMocker : BaseMockerBuilder<DefaultLockstepStepEventService>
    {
        public Mocker<IMessageBus> MessageBusMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }

        public DefaultLockstepStepEventService DefaultLockstepStepEventService => this.GetInstance();

        public DefaultLockstepStepEventServiceMocker()
        {
            this.MessageBusMocker = new Mocker<IMessageBus>();
            this.LoggerMocker = new Mocker<ILogger>();
        }

        public static PredictiveStepEventServiceMocker Create()
        {
            return new PredictiveStepEventServiceMocker();
        }

        protected override DefaultLockstepStepEventService Build()
        {
            return new DefaultLockstepStepEventService(
                this.MessageBusMocker.GetInstance(),
                this.LoggerMocker.GetInstance());
        }
    }
}
