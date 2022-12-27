using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Messages
{
    public sealed class SimulationStateTick : Message
    {
        public readonly Tick Tick;

        public SimulationStateTick(Tick tick)
        {
            this.Tick = tick;
        }
    }
}
