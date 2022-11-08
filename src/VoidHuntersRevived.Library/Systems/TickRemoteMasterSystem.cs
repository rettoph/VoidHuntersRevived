using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationSystem(NetAuthorization.Master)]
    internal sealed class TickRemoteMasterSystem : ISystem, ISubscriber<Tick>
    {
        private readonly NetScope _netScope;
        private readonly ITickService _ticks;
        private readonly ITickFactory _tickFactory;

        public TickRemoteMasterSystem(
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
            if(newUser.NetPeer is null)
            {
                return;
            }

            if(_ticks.Current is null)
            {
                return;
            }

            // Enqueue a new user joined action for the new user.
            _tickFactory.Enqueue(new UserPilot(
                user: newUser.CreateAction(UserAction.Actions.UserJoined, ClaimAccessibility.Public)));

            // Send the current game state to the new user
            _netScope.Create<GameState>(
                body: new GameState(
                    nextTickId: _ticks.Current.Id + 1,
                    history: _ticks.History
                )
            ).AddRecipient(newUser.NetPeer).Enqueue();
        }

        public void Process(in Tick tick)
        {
            _netScope.Create<Tick>(in tick)
                .AddRecipients(_netScope.Users.Peers)
                .Enqueue();
        }
    }
}
