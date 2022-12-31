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
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Slave)]
    internal sealed class LockstepCurrentUserRemoteSlaveSystem : ISystem, ILockstepSimulationSystem, 
        ISubscriber<DirectionInput>
    {
        private readonly IBus _bus;
        private readonly NetScope _scope;

        public LockstepCurrentUserRemoteSlaveSystem(NetScope scope, IBus bus)
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
            GC.SuppressFinalize(this);

            _bus.Unsubscribe(this);
        }

        public void Process(in DirectionInput message)
        {
            _scope.Create(in message).Enqueue();
        }
    }
}
