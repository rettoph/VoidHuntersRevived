using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationFilter(NetAuthorization.Slave)]
    internal sealed class GameStateRemoteSlaveSystem : ISystem, 
        ISubscriber<INetIncomingMessage<GameStateTick>>, 
        ISubscriber<INetIncomingMessage<GameStateEnd>>
    {
        private readonly GameState _state;

        public GameStateRemoteSlaveSystem(GameState state, ILogger log)
        {
            _state = state;
            _state.BeginRead();
        }

        public void Initialize(ECSWorld world)
        {
            // throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public void Process(in INetIncomingMessage<GameStateTick> message)
        {
            _state.Read(message.Body.Tick);
        }

        public void Process(in INetIncomingMessage<GameStateEnd> message)
        {
            _state.Read(Tick.Empty(message.Body.LastTickId));
            _state.EndRead();
        }
    }
}
