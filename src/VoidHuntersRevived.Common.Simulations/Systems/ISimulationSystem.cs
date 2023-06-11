using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface ISimulationSystem : ISystem
    {
        void Initialize(ISimulation simulation);
    }
}
