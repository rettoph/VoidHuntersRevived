using Guppy.Attributes;
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
        private bool _ready;
        private List<Tick> _history;
        private int _historyPosition;
        private uint _historyId;
        private bool _gameStateRecieved;

        public RemoteTickProvider(TickBuffer buffer)
        {
            _buffer = buffer;
            _next = Tick.Default;
            _history = new List<Tick>();
        }

        public void Update(GameTime gameTime)
        {
            //
        }

        public bool Ready()
        {
            if(_ready)
            {
                var result = _buffer.TryPop(out _next);
                return result;
            }

            if(!_gameStateRecieved)
            {
                return false;
            }

            if(_history.Count == 0)
            {
                _ready = true;
                return false;
            }

            return true;
        }

        public Tick Next()
        {
            if(_ready)
            {
                return _next!;
            }

            Tick next;
            
            if(_historyPosition < _history.Count)
            {
                next = _history[_historyPosition];

                if (_historyId != next.Id)
                {
                    next = Tick.Empty(_historyId);
                    _historyId++;

                    return next;
                }

                _historyPosition++;
                _historyId++;

                return next;
            }

            if(_historyId + 1 >= _buffer.NextId)
            {
                _ready = true;
            }

            next = Tick.Empty(_historyId);
            _historyId++;

            return next;
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
            _history.AddRange(message.Body.History);
            _gameStateRecieved = true;
        }
    }
}
