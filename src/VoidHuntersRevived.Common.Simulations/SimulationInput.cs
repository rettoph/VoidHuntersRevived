using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public class SimulationInput : ISimulationEventData
    {
        public required ParallelKey Key { get; init;  }
        public required int SenderId { get; init; }
    }
}
