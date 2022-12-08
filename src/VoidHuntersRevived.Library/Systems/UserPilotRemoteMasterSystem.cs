using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Identity;
using Guppy.Network.Messages;
using Guppy.Network;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using Guppy.Network.Extensions.Identity;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Services;
using MonoGame.Extended.Entities;
using Guppy.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Library.Attributes;
using LiteNetLib;
using Serilog;

namespace VoidHuntersRevived.Library.Systems
{
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class UserPilotRemoteMasterSystem : ISystem
    {
        private readonly NetScope _netScope;
        private readonly ITickFactory _tickFactory;
        private readonly GameState _state;
        private readonly ILogger _log;

        public UserPilotRemoteMasterSystem(
            NetScope netScope,
            GameState state,
            ITickFactory tickFactory,
            ILogger log)
        {
            _netScope = netScope;
            _state = state;
            _tickFactory = tickFactory;
            _log = log;
        }

        public void Initialize(World world)
        {
            _netScope.Users.OnUserJoined += this.HandleUserJoined;
        }

        public void Dispose()
        {
            _netScope.Users.OnUserJoined -= this.HandleUserJoined;
        }

        private void HandleUserJoined(IUserService sender, User newUser)
        {
            if (newUser.NetPeer is null)
            {
                return;
            }

            // Enqueue a new user joined action for the new user.
            _tickFactory.Enqueue(new UserPilot(
                user: newUser.CreateAction(
                    action: UserAction.Actions.UserJoined,
                    accessibility: ClaimAccessibility.Public)));

            var lastTickId = _state.LastTickId;

            _log.Verbose($"New User Connected - Sending GameState at TickId: {lastTickId}");

            for (var i = 0; i < _state.History.Count; i++)
            {
                var tick = _state.History[i];
            
                if (tick.Id > lastTickId)
                {
                    break;
                }

                _log.Verbose($"Sending TickId: {tick.Id}");

                var om = _netScope.Create<GameStateTick>(new GameStateTick(tick))
                    .AddRecipient(newUser.NetPeer)
                    .Enqueue();
            }

            _log.Verbose($"Sending GameStateEnd: {lastTickId}");
            _netScope.Create(new GameStateEnd(lastTickId))
                .AddRecipient(newUser.NetPeer)
                .Enqueue();
        }
    }
}
