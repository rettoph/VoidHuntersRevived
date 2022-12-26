using Guppy.Common;
using Guppy.Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    public abstract class Simulation
    {
        public readonly IBus Bus;

        protected Simulation()
        {
        }
    }
}
