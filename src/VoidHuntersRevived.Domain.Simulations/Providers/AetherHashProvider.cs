using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Providers;

namespace VoidHuntersRevived.Domain.Simulations.Providers
{
    internal sealed class AetherHashProvider : ISimulationHashProvider
    {
        public void GetHash(ISimulation simulation, out int hash)
        {
            throw new NotImplementedException();
        }
    }
}
