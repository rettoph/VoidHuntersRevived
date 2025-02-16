using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class LockstepStepEventServiceMocker
    {
        public Mocker<INetScope<IStrategy>> NetScope { get; set; }
        public Mocker<IMessageBus> MessageBusMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }
        public readonly ClientLockstepStepEventService LockstepEventService;

        public LockstepStepEventServiceMocker()
        {
            this.NetScope = new Mocker<INetScope<IStrategy>>();
            this.MessageBusMocker = new Mocker<IMessageBus>();
            this.LoggerMocker = new Mocker<ILogger>();
            this.LockstepEventService = new ClientLockstepStepEventService(
                this.NetScope.GetInstance(),
                this.MessageBusMocker.GetInstance(),
                this.LoggerMocker.GetInstance());
        }

        public static PredictiveStepEventServiceMocker Create()
        {
            return new PredictiveStepEventServiceMocker();
        }
    }
}
