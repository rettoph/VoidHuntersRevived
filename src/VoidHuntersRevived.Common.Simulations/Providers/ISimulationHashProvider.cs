using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Providers
{
    public interface ISimulationHashProvider
    {
        void GetHash(ISimulation simulation, out int hash);
    }
}
