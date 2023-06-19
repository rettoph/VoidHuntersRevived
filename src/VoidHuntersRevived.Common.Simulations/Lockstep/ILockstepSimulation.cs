using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public interface ILockstepSimulation : ISimulation
    {
        Tick CurrentTick { get; }

        event OnEventDelegate<Tick>? OnTick;
    }
}
