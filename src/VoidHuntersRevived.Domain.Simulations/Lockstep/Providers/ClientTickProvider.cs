using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
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
