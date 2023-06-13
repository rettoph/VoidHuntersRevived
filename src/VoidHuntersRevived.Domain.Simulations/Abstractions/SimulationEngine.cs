using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    [AllowMultiple]
    internal sealed class SimulationEngine : IGetReadyEngine
    {
        private readonly ISimulationSystem _system;
        private readonly ISimulation _simulation;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public SimulationEngine(ISimulation simulation, ISimulationSystem system)
        {
            _system = system;
            _simulation = simulation;
        }

        public void Ready()
        {
            _system.Initialize(_simulation);
        }
    }
}
