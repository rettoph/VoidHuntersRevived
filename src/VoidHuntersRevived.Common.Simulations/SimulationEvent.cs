using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public class SimulationEvent : Message<SimulationEvent>
    {
        public readonly ISimulation Simulation;
        public readonly ISimulationEventData Data;
        public readonly SimulationEventResult Result;

        public SimulationEvent(ISimulation simulation, ISimulationEventData data, SimulationEventResult result)
        {
            this.Simulation = simulation;
            this.Data = data;
            this.Result = result;
        }
    }
}
