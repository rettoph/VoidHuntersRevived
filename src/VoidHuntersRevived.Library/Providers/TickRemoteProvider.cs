using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Network;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Providers
{
    [AutoSubscribe]
    internal sealed class TickRemoteProvider : ITickProvider,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<ToggleSimulatedLag>
    {
        private readonly GameState _state;
        private readonly TickBuffer _buffer;
        private bool _lagging;


        public int AvailableId { get; private set; }

        public TickProviderStatus Status { get; private set; }

        public TickRemoteProvider(GameState state)
        {
            _state = state;
            _buffer = new TickBuffer();

            this.Status = TickProviderStatus.Historical;
        }

        public bool TryGetNextTick([MaybeNullWhen(false)] out Tick next)
        {
            return _buffer.TryPop(_state.NextTickId, out next);
        }

        void ISubscriber<INetIncomingMessage<Tick>>.Process(in INetIncomingMessage<Tick> message)
        {
            _buffer.Enqueue(message.Body);

            if(_lagging)
            {
                return;
            }

            this.AvailableId = _buffer.Tail?.Id ?? this.AvailableId;
        }

        public void Process(in ToggleSimulatedLag message)
        {
            _lagging = !_lagging;

            if(!_lagging)
            {
                this.AvailableId = _buffer.Tail?.Id ?? this.AvailableId;
            }
        }
    }
}
