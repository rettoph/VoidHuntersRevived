using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface ISimulationSystem : ISystem
    {
        void Initialize(IParallelComponentMapperService components, IParallelEntityService entities);
        void Initialize(ISimulation simulation);
    }
}
