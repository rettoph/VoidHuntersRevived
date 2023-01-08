using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    [Flags]
    public enum SimulationType
    {
        None = 0,
        Lockstep = 1,
        Predictive = 2
    }
}
