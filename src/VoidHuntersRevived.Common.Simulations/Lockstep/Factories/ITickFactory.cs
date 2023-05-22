using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep.Factories
{
    public interface ITickFactory
    {
        void Enqueue(SimulationEventData input);
        Tick Create(int id);
        void Reset();
    }
}
