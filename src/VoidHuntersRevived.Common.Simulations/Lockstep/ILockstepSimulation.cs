using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Events;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public interface ILockstepSimulation : ISimulation
    {
        Tick CurrentTick { get; }

        IEnumerable<Tick> History { get; }

        event OnEventDelegate<EventDto>? OnEvent;
    }
}
