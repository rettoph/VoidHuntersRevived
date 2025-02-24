using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class DefaultLockstepStepEventServiceMocker : MockBuilder<DefaultLockstepStepEventService>
    {
        public Mocker<IMessageBus> MessageBusMocker { get; init; }
        public Mocker<ILogger> LoggerMocker { get; init; }

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
                this.MessageBusMocker.Object,
                this.LoggerMocker.Object);
        }
    }
}
