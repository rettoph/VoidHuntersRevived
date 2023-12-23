using Guppy.Attributes;
using Guppy.Messaging;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Serilog;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class LockstepClient_TickEngine : BasicEngine,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<TickHistoryStart>>,
        ISubscriber<INetIncomingMessage<TickHistoryItem>>,
        ISubscriber<INetIncomingMessage<TickHistoryEnd>>
    {
        private readonly TickBuffer _ticks;
        private readonly ILogger _logger;

        public LockstepClient_TickEngine(ILogger logger, TickBuffer ticks)
        {
            _ticks = ticks;
            _logger = logger;
        }

        public void Process(in Guid messsageId, INetIncomingMessage<Tick> message)
        {
            _ticks.Enqueue(message.Body);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryStart> message)
        {
            //_ticks.Clear();
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryItem> message)
        {
            for (int id = (_ticks.Tail?.Id ?? _ticks.Popped?.Id ?? 0) + 1; id < message.Body.Tick.Id; id++)
            {
                _ticks.Enqueue(Tick.Empty(id));
            }

            _ticks.Enqueue(message.Body.Tick);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryEnd> message)
        {
            _logger.Warning($"{message.Body.CurrentTickId}");
            for (int id = (_ticks.Tail?.Id ?? _ticks.Popped?.Id ?? 0) + 1; id < message.Body.CurrentTickId; id++)
            {
                _ticks.Enqueue(Tick.Empty(id));
            }
        }
    }
}
