using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface ISimulationEngine<TSimulation> : IEngine, IGetReadyEngine
        where TSimulation : ISimulation
    {
        void Initialize(TSimulation simulation);
    }

    public interface ISimulationEngine : ISimulationEngine<ISimulation>
    {
        
    }
}
