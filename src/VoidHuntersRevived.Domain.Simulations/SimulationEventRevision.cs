using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationEventRevision<TSimulationEventData> : Message<ISimulationEventRevision<TSimulationEventData>>, ISimulationEventRevision<TSimulationEventData>
        where TSimulationEventData : class
    {
        public required ParallelKey Key { get; init; }

        public int SenderId { get; init; }

        public required ISimulation Simulation { get; init; }

        public required TSimulationEventData Body { get; init; }

        public required object? Response { get; init; }

        object ISimulationEventRevision.Body => this.Body;
    }
}
