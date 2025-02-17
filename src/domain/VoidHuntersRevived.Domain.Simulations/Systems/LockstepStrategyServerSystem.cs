using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class LockstepStrategyServerSystem(
        IStepEventService eventService
    ) : ISceneSystem,
        IInitializeSystem,
        INetIncomingMessageSubscriber<EnqueuedStepInput>,
        ISubscriber<SubscriberSequenceGroupEnum, EnqueuedStepInput>
    {
        private readonly IStepEventService _eventService = eventService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PostInitialize)]
        public void Initialize()
        {
            this._eventService.Input(NameSpace<LockstepStrategyServerSystem>.Instance, new Simulation_Begin());
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
