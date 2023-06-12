using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    [AutoLoad]
    internal sealed class LockstepClient_TickSystem : BasicSystem,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<TickHistoryStart>>,
        ISubscriber<INetIncomingMessage<TickHistoryItem>>,
        ISubscriber<INetIncomingMessage<TickHistoryEnd>>
    {
        private readonly TickBuffer _ticks;

        public LockstepClient_TickSystem(TickBuffer ticks)
        {
            _ticks = ticks;
        }

        public void Process(in INetIncomingMessage<Tick> message)
        {
            _ticks.Enqueue(message.Body);
        }

        public void Process(in INetIncomingMessage<TickHistoryStart> message)
        {
            _ticks.Clear();
        }

        public void Process(in INetIncomingMessage<TickHistoryItem> message)
        {
            for (int id = (_ticks.Tail?.Id ?? _ticks.Popped?.Id ?? 0) + 1; id < message.Body.Tick.Id - 1; id++)
            {
                _ticks.Enqueue(Tick.Empty(id));
            }

            _ticks.Enqueue(message.Body.Tick);
        }

        public void Process(in INetIncomingMessage<TickHistoryEnd> message)
        {
            for (int id = (_ticks.Tail?.Id ?? _ticks.Popped?.Id ?? 0) + 1; id < message.Body.CurrentTickId; id++)
            {
                _ticks.Enqueue(Tick.Empty(id));
            }
        }
    }
}
