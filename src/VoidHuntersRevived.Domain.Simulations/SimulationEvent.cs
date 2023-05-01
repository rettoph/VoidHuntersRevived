using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationEvent<TSimulationEventData> : ISimulationEvent<TSimulationEventData>
        where TSimulationEventData : class
    {
        private ParallelKeyFactory? _keyFactory;

        public required ParallelKey Key { get; init; }

        public int SenderId { get; init; }

        public required ISimulation Simulation { get; init; }

        public required TSimulationEventData Body { get; init; }

        public ParallelKeyFactory KeyFactory => _keyFactory ??= new ParallelKeyFactory(this.Key);

        public SimulationEventResult Result { get; internal set; }

        object ISimulationEvent.Body => this.Body;
    }
}
