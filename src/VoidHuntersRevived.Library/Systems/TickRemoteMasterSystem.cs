using Guppy.Attributes;
using Guppy.Common;
using Guppy.ECS.Attributes;
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
    [AutoLoad]
    [GuppySystem(typeof(GameGuppy))]
    [NetAuthorizationSystem(NetAuthorization.Master)]
    internal sealed class TickRemoteMasterSystem : ISystem, ISubscriber<Tick>
    {
        private NetScope _netScope;
        private IBus _bus;
        private ITickService _ticks;
        private ITickFactory _tickFactory;

        public TickRemoteMasterSystem(
            NetScope netScope,
            IBus bus,
            ITickService ticks,
            ITickFactory tickFactory)
        {
            _netScope = netScope;
            _bus = bus;
            _ticks = ticks;
            _tickFactory = tickFactory;
        }

        public void Initialize(World world)
        {
            _bus.Subscribe(this);
            _netScope.Users.OnUserJoined += this.HandleUserJoined;
        }

        public void Dispose()
        {
            _bus.Unsubscribe(this);
            _netScope.Users.OnUserJoined -= this.HandleUserJoined;
        }

        private void HandleUserJoined(IUserService sender, User newUser)
        {
            if(newUser.NetPeer is null)
            {
                return;
            }

            // Enqueue a new user joined action for the new user.
            _tickFactory.Enqueue(new UserPilot(
                user: newUser.CreateAction(UserAction.Actions.UserJoined, ClaimAccessibility.Public)));

            // Send the current game state to the new user
            _netScope.Create<GameState>(new GameState()
            {
                NextTickId = _ticks.Current.Id + 1
            }).AddRecipient(newUser.NetPeer).Enqueue();
        }

        public void Process(in Tick tick)
        {
            _netScope.Create<Tick>(in tick)
                .AddRecipients(_netScope.Users.Peers)
                .Enqueue();
        }
    }
}
