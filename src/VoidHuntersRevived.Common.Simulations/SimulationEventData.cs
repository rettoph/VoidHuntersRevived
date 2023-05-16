using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public class SimulationEventData : Message<SimulationEventData>
    {
        public required ParallelKey Key { get; init;  }

        public required int SenderId { get; init; }

        public required object Body { get; init; }
    }
}
