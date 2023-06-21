using Guppy.Attributes;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages.Commands;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class Lockstep_TickEngine : BasicEngine<ILockstepSimulation>,
        ISubscriber<Ticks>
    {
        public void Process(in Guid messageId, in Ticks message)
        {
            foreach(Tick historical in this.Simulation.History)
            {
                Console.WriteLine($"Id: {historical.Id}, Events: {historical.Events.Length}, Hash: {historical.Hash}");
            }
        }
    }
}
