using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class LockstepEventService(IMessageBus messageBus, ILogger logger) : BaseEventService(messageBus, logger)
    {
        private readonly IMessageBus _messageBus = messageBus;

        public override void Input(EnqueuedStepInput input)
        {
            this._messageBus.Publish<SubscriberSequenceGroupEnum, EnqueuedStepInput>(input);
        }
    }
}
