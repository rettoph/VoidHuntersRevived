using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Network;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Providers
{
    [AutoSubscribe]
    internal sealed class TickRemoteProvider : ITickProvider,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<GameState>>
    {
        private TickBuffer _buffer;

        public int CurrentId { get; set; }

        public int LastId { get; private set; }

        public TickRemoteProvider()
        {
            _buffer = new TickBuffer(0);
        }

        public bool Next([MaybeNullWhen(false)] out Tick next)
        {
            return _buffer.TryPop(out next);
        }

        void ISubscriber<INetIncomingMessage<Tick>>.Process(in INetIncomingMessage<Tick> message)
        {
            if(message.DeliveryMethod == DeliveryMethod.ReliableOrdered)
            {
                return;
            }
            _buffer.Enqueue(message.Body);

            this.LastId = _buffer.Tail?.Id ?? this.LastId;
        }

        void ISubscriber<INetIncomingMessage<GameState>>.Process(in INetIncomingMessage<GameState> message)
        {
            if(message.Body.Type == GameStateType.Begin)
            {
                _buffer.CurrentId = message.Body.LastHistoricTickId;
            }
        }
    }
}
