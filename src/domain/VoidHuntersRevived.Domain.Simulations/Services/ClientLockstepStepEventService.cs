using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class ClientLockstepStepEventService(
        INetScope<IStrategy> netScope,
        IMessageBus messageBus,
        ILogger logger
    ) : BaseStepEventService(messageBus, logger)
    {
        private readonly INetScope<IStrategy> _netScope = netScope;

        public override void Input(EnqueuedStepInput input)
        {
            this._netScope.CreateMessage(input);
        }
    }
}
