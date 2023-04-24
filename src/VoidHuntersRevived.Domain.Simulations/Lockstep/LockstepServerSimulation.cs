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
using static VoidHuntersRevived.Domain.Simulations.Simulation;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class LockstepServerSimulation : LockstepSimulation,
        ISubscriber<INetIncomingMessage<ClientInputRequest>>
    {
        private readonly ITickFactory _ticks;

        public LockstepServerSimulation(
            State state, 
            IBus bus,
            ITickFactory ticks,
            IStepService steps, 
            IParallelableService parallelables, 
            IGlobalSimulationService globalSimulationService) : base(state, bus, steps, parallelables, globalSimulationService)
        {
            _ticks = ticks;
        }

        public override void Input(ParallelKey sender, IData data)
        {
            _ticks.Enqueue(new EventDto(
                sender: sender,
                data: data));
        }

        public void Process(in INetIncomingMessage<ClientInputRequest> message)
        {
            if (message.Peer is null)
            {
                return;
            }

            this.Input(message.Peer.GetKey(), message.Body.Input);
        }
    }
}
