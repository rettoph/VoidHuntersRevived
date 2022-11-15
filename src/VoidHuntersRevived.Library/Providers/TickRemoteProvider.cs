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
        private delegate bool NextDelegate([MaybeNullWhen(false)] out Tick next);

        private readonly TickBuffer _buffer;
        private NextDelegate _next;
        private GameStateType _gameStateType;
        private int _lastHistoricTickId;

        public int CurrentId { get; set; }

        public int LastId { get; private set; }

        public TickRemoteProvider()
        {
            _buffer = new TickBuffer(Tick.MinimumValidId - 1);
            _next = this.NextHistoric;
        }

        public bool Next([MaybeNullWhen(false)] out Tick next)
        {
            return _next(out next);
        }

        private bool NextHistoric([MaybeNullWhen(false)] out Tick next)
        {
            if(_gameStateType == GameStateType.None)
            {
                return FalseResponse(out next);
            }

            if(_buffer.Head is null)
            {
                return FalseResponse(out next);
            }

            if (_buffer.CurrentId < _lastHistoricTickId && _buffer.TryPop(out next))
            {
                return true;
            }

            if(_buffer.CurrentId == _lastHistoricTickId)
            {
                _next = _buffer.TryPop;
                return _buffer.TryPop(out next);
            }

            next = Tick.Empty(++_buffer.CurrentId);
            return true;
        }

        private static bool FalseResponse([MaybeNullWhen(false)] out Tick next)
        {
            next = null;
            return false;
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
            _gameStateType = message.Body.Type;

            if (_gameStateType == GameStateType.Begin)
            {
                _lastHistoricTickId = message.Body.LastHistoricTickId;
            }
        }
    }
}
