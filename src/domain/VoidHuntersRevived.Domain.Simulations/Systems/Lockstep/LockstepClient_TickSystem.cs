using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class LockstepClient_TickSystem(
        ILogger logger,
        ClientLinkedListLockstepTickService tickService
    ) : ISceneSystem,
        INetIncomingMessageSubscriber<Tick>,
        INetIncomingMessageSubscriber<TickHistoryStart>,
        INetIncomingMessageSubscriber<TickHistoryItem>,
        INetIncomingMessageSubscriber<TickHistoryEnd>
    {
        private readonly ClientLinkedListLockstepTickService _tickService = tickService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<Tick> message)
        {
            EnqueueTickResponseEnum response = this._tickService.TryEnqueue(message.Body);
            this._logger.Verbose("Tick. Attempted to enqueue Tick {Id}, Response = {Response}", message.Body.Id, response);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<TickHistoryStart> message)
        {
            //this._tickService.Reset();
            this._logger.Verbose("TickHistoryStart. CurrentTickId = {CurrentTickId}", message.Body.CurrentTickId);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<TickHistoryItem> message)
        {
            Tick? previous = this._tickService.Previous(message.Body.Tick.Id);
            int id = (previous?.Id ?? 0) + 1;

            this._logger.Verbose("TickHistoryItem. TickId = {CurrentTickId}, PreviousTickId = {PreviousTickId}", message.Body.Tick.Id, previous?.Id ?? 0);
            EnqueueTickResponseEnum response;
            for (; id < message.Body.Tick.Id; id++)
            {
                response = this._tickService.TryEnqueue(Tick.Empty(id));
                this._logger.Verbose("TickHistoryItem. Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }

            response = this._tickService.TryEnqueue(message.Body.Tick);
            this._logger.Verbose("TickHistoryItem. Attempted to enqueue Tick {TickId}, Response = {Response}", id, response);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<TickHistoryEnd> message)
        {
            Tick? previous = this._tickService.Previous(message.Body.CurrentTickId);
            int id = (previous?.Id ?? 0) + 1;

            this._logger.Verbose("TickHistoryEnd. CurrentTickId = {CurrentTickId}, PreviousId = {PreviousId}", message.Body.CurrentTickId, previous?.Id ?? 0);
            for (; id < message.Body.CurrentTickId; id++)
            {
                EnqueueTickResponseEnum response = this._tickService.TryEnqueue(Tick.Empty(id));
                this._logger.Verbose("TickHistoryEnd. Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }
        }
    }
}