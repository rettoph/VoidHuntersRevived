using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Network;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Models;

namespace VoidHuntersRevived.Library.Providers
{
    internal sealed class RemoteTickProvider : ITickProvider, IDisposable,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<GameState>>
    {
        private IBus _bus;
        private TickBuffer _buffer;
        private Tick _next;

        public RemoteTickProvider(TickBuffer buffer, IBus bus)
        {
            _bus = bus;
            _buffer = buffer;
            _next = Tick.Default;

            _bus.Subscribe<INetIncomingMessage<Tick>>(this);
            _bus.Subscribe<INetIncomingMessage<GameState>>(this);
        }

        public void Dispose()
        {
            _bus.Unsubscribe<INetIncomingMessage<Tick>>(this);
            _bus.Unsubscribe<INetIncomingMessage<GameState>>(this);
        }

        public void Update(GameTime gameTime)
        {
            //
        }

        public bool Ready()
        {
            var result = _buffer.TryPop(out _next);
            return result;
        }

        public Tick Next()
        {
            return _next!;
        }

        void ISubscriber<INetIncomingMessage<Tick>>.Process(in INetIncomingMessage<Tick> message)
        {
            var tick = message.Body;
            _buffer.Enqueue(message.Body);

            Console.WriteLine($"Recieved Tick {tick.Id}, Waiting for {_buffer.NextId}.");
        }

        void ISubscriber<INetIncomingMessage<GameState>>.Process(in INetIncomingMessage<GameState> message)
        {
            // Begin listening for new ticks
            _buffer.NextId = message.Body.NextTickId;
        }
    }
}
