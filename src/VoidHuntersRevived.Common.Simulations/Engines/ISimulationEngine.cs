using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface ISimulationEngine : IEngine
    {
        void Initialize(ISimulation simulation);
    }
}
