﻿using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class LockstepClientSimulation : LockstepSimulation
    {
        private NetScope _network;

        public LockstepClientSimulation(
            State state, 
            NetScope network,
            IInputService input,
            IStepService steps, 
            IParallelableService parallelables, 
            IGlobalSimulationService globalSimulationService) : base(state, input, steps, parallelables, globalSimulationService)
        {
            _network = network;
        }

        public override void Input(Input input)
        {
            _network.Messages.Create(new InputRequest(input)).Enqueue();
        }
    }
}
