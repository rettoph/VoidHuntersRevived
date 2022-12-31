using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Enums;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class LockstepAetherDebugRemoteMasterSystem : ISystem, ILockstepSimulationSystem, ISubscriber<Tick>
    {
        private readonly ITickFactory _tickFactory;
        private readonly ISimulationService _simulations;

        public LockstepAetherDebugRemoteMasterSystem(ISimulationService simulations, ITickFactory tickFactory)
        {
            _simulations = simulations;
            _tickFactory = tickFactory;
        }

        public void Initialize(World world)
        {
        }

        public void Dispose()
        {
        }

        public void Process(in Tick message)
        {
            foreach (var body in _simulations[SimulationType.Lockstep].Aether.BodyList)
            {
                _tickFactory.Enqueue(new BodyPosition(body.GetHashCode(), body.Position, SimulationType.Lockstep));
            }
        }
    }
}
