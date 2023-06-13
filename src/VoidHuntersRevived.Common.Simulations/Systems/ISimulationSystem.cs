using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface ISimulationSystem : ISystem
    {
        void Initialize(ISimulation simulation);
    }
}
