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

        public int CurrentId => _buffer.CurrentId;

        public int AvailableId { get; private set; }

        public TickProviderStatus Status { get; private set; }

        public TickRemoteProvider()
        {
            _buffer = new TickBuffer(Tick.MinimumValidId - 1);
            _next = this.NextHistoric;

            this.Status = TickProviderStatus.Historical;
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
                this.Status = TickProviderStatus.Realtime;
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
            _buffer.Enqueue(message.Body);

            this.AvailableId = _buffer.Tail?.Id ?? this.AvailableId;
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
