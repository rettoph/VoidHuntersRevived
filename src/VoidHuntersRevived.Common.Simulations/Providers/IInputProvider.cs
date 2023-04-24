using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Providers
{
    public interface IInputProvider
    {
        IInput Get(Guid id, ISimulation simulation);

        IInput Create(Guid id, ISimulation simulation, ParallelKey sender, IData data);
    }
}
