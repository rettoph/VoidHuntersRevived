using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Factories
{
    public interface ITickFactory
    {
        void Enqueue(ISimulationInputData data);

        Tick Create(int id);

        void Reset();
    }
}
