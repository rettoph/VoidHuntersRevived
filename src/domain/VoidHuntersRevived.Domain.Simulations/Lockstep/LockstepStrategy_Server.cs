using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Server(
        ISettingService settings,
        IGuppyScope scope,
        DefaultLockstepStepEventService eventService,
        ILogger logger
    ) : LockstepStrategy(settings, scope, eventService, logger),
        INetIncomingMessageSubscriber<EnqueuedStepInput>,
        ISubscriber<SubscriberSequenceGroupEnum, EnqueuedStepInput>
    {
        private readonly DefaultLockstepStepEventService _eventService = eventService;

        protected override void Initialize()
        {
            base.Initialize();

            this.Events.Input(NameSpace<LockstepStrategy_Server>.Instance, new Simulation_Begin());
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<EnqueuedStepInput> message)
        {
            this._eventService.Input(message.Body);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(EnqueuedStepInput message)
        {
            this._eventService.Input(message);
        }
    }
}