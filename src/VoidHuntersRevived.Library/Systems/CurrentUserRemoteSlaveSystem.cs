using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity.Providers;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoLoad]
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationSystem(NetAuthorization.Slave)]
    internal sealed class CurrentUserRemoteSlaveSystem : ISystem, ISubscriber<DirectionInput>
    {
        private IBus _bus;
        private NetScope _scope;

        public CurrentUserRemoteSlaveSystem(NetScope scope, IBus bus)
        {
            _scope = scope;
            _bus = bus;
        }

        public void Initialize(World world)
        {
            _bus.Subscribe(this);
        }

        public void Dispose()
        {
            _bus.Unsubscribe(this);
        }

        public void Process(in DirectionInput message)
        {
            _scope.Create<DirectionInput>(in message).Enqueue();
        }
    }
}
