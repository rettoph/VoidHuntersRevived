using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations.Providers;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationEvent<TSimulationEventData> : ISimulationEvent<TSimulationEventData>
        where TSimulationEventData : class
    {
        public required ParallelKey Key { get; init; }

        public int SenderId { get; init; }

        public required ISimulation Simulation { get; init; }

        public required TSimulationEventData Body { get; init; }

        public SimulationEventResult Result { get; internal set; }

        object ISimulationEvent.Body => this.Body;

        public ParallelKey NewKey()
        {
            return this.Simulation.Keys.Next(this.Key);
        }

        public ParallelKey PreviousKey()
        {
            return this.Simulation.Keys.Previous(this.Key);
        }
    }
}
