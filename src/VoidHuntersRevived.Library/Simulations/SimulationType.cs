using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    [Flags]
    public enum SimulationType
    {
        None = 0,
        Predictive = 1,
        Lockstep = 2
    }
}
