using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Simulations.Lockstep.Utilities;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Client)]
    internal class ClientTickProvider : ITickProvider,
        ISubscriber<INetIncomingMessage<Tick>>
    {
        private readonly State _state;
        private readonly TickBuffer _buffer;
        private bool _lagging;

        public int AvailableId { get; private set; }

        public ClientTickProvider(State state)
        {
            _state = state;
            _buffer = new TickBuffer();
        }

        public bool TryGetNextTick([MaybeNullWhen(false)] out Tick next)
        {
            return _buffer.TryPop(_state.NextTickId, out next);
        }

        void ISubscriber<INetIncomingMessage<Tick>>.Process(in INetIncomingMessage<Tick> message)
        {
            _buffer.Enqueue(message.Body);

            if (_lagging)
            {
                return;
            }

            this.AvailableId = _buffer.Tail?.Id ?? this.AvailableId;
        }
    }
}
