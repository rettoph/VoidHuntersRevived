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

namespace VoidHuntersRevived.Library.Systems
{
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class UserPilotRemoteMasterSystem : ISystem
    {
        private readonly NetScope _netScope;
        private readonly ITickService _ticks;
        private readonly ITickFactory _tickFactory;

        public UserPilotRemoteMasterSystem(
            NetScope netScope,
            ITickService ticks,
            ITickFactory tickFactory)
        {
            _netScope = netScope;
            _ticks = ticks;
            _tickFactory = tickFactory;
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

            if (_ticks.Current is null)
            {
                return;
            }

            // Enqueue a new user joined action for the new user.
            _tickFactory.Enqueue(new UserPilot(
                user: newUser.CreateAction(
                    action: UserAction.Actions.UserJoined, 
                    accessibility: ClaimAccessibility.Public)));

            // Send the current game state to the new user
            _netScope.Create<GameState>(
                body: new GameState(
                    nextTickId: _ticks.Current.Id + 1,
                    history: _ticks.History
                )
            ).AddRecipient(newUser.NetPeer).Enqueue();
        }
    }
}
