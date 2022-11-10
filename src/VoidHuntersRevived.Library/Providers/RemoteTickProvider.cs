using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Network;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Providers
{
    [AutoSubscribe]
    internal sealed class RemoteTickProvider : ITickProvider,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<GameState>>
    {
        private readonly TickBuffer _buffer;
        private Tick _next;

        public RemoteTickProvider(TickBuffer buffer)
        {
            _buffer = buffer;
            _next = Tick.Default;
        }

        public void Update(GameTime gameTime)
        {
            //
        }

        public bool Next([MaybeNullWhen(false)] out Tick next)
        {
            return _buffer.TryPop(out next);
        }

        void ISubscriber<INetIncomingMessage<Tick>>.Process(in INetIncomingMessage<Tick> message)
        {
            var tick = message.Body;
            _buffer.Enqueue(tick);
        }

        void ISubscriber<INetIncomingMessage<GameState>>.Process(in INetIncomingMessage<GameState> message)
        {
            // Begin listening for new ticks
            _buffer.NextId = message.Body.NextTickId;
        }
    }
}
