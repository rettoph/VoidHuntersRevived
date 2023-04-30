using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class LockstepServerSimulation : LockstepSimulation
    {
        private readonly ITickFactory _ticks;

        public LockstepServerSimulation(
            State state, 
            ISimulationEventPublishingService input,
            ITickFactory ticks,
            IStepService steps, 
            IParallelableService parallelables, 
            IGlobalSimulationService globalSimulationService) : base(state, input, steps, parallelables, globalSimulationService)
        {
            _ticks = ticks;
        }

        public override void Input(SimulationInput input)
        {
            _ticks.Enqueue(input);
        }
    }
}
