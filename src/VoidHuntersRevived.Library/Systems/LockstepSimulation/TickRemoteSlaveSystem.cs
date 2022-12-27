using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Services;
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

namespace VoidHuntersRevived.Library.Systems.LockstepSimulation
{
    [NetAuthorizationFilter(NetAuthorization.Slave)]
    internal sealed class TickRemoteSlaveSystem : ISystem, ILockstepSimulationSystem, ISubscriber<Tick>
    {
        private NetScope _netScope;
        private IBus _bus;
        private ITickService _ticks;
        private ITickFactory _tickFactory;

        public TickRemoteSlaveSystem(
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
        }

        public void Dispose()
        {
            _bus.Unsubscribe(this);
        }


        public void Process(in Tick tick)
        {
        }
    }
}
