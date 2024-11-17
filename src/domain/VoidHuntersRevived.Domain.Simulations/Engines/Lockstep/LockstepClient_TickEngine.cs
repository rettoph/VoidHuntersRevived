using Guppy.Core.Common.Attributes;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Engine.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    [LoggerContext("TickEngine")]
    internal class LockstepClient_TickEngine(ILogger logger, TickBuffer ticks) : StrategyEngine,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<TickHistoryStart>>,
        ISubscriber<INetIncomingMessage<TickHistoryItem>>,
        ISubscriber<INetIncomingMessage<TickHistoryEnd>>
    {
        private readonly TickBuffer _ticks = ticks;
        private readonly ILogger _logger = logger;

        public void Process(in Guid messsageId, INetIncomingMessage<Tick> message)
        {
            TickBuffer.EnqueueTickResponse response = _ticks.TryEnqueue(message.Body);
            _logger.Verbose("Attempted to enqueue Tick {Id}, Response = {Response}", message.Body.Id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryStart> message)
        {
            //_ticks.Clear();
            _logger.Verbose("CurrentTickId = {CurrentTickId}", message.Body.CurrentTickId);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryItem> message)
        {
            TickBuffer.EnqueueTickResponse response = TickBuffer.EnqueueTickResponse.NotEnqueued;
            Tick? previous = _ticks.Previous(message.Body.Tick.Id);
            int id = (previous?.Id ?? 0) + 1;

            _logger.Verbose("TickId = {CurrentTickId}, PreviousTickId = {PreviousTickId}", message.Body.Tick.Id, previous?.Id ?? 0);
            for (; id < message.Body.Tick.Id; id++)
            {
                response = _ticks.TryEnqueue(Tick.Empty(id));
                _logger.Verbose("Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }

            response = _ticks.TryEnqueue(message.Body.Tick);
            _logger.Verbose("Attempted to enqueue Tick {TickId}, Response = {Response}", id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryEnd> message)
        {
            TickBuffer.EnqueueTickResponse response = TickBuffer.EnqueueTickResponse.NotEnqueued;
            Tick? previous = _ticks.Previous(message.Body.CurrentTickId);
            int id = (previous?.Id ?? 0) + 1;

            _logger.Verbose("CurrentTickId = {CurrentTickId}, PreviousId = {PreviousId}", message.Body.CurrentTickId, previous?.Id ?? 0);
            for (; id < message.Body.CurrentTickId; id++)
            {
                response = _ticks.TryEnqueue(Tick.Empty(id));
                _logger.Verbose("Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }
        }
    }
}
