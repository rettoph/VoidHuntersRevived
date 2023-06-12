using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    [AllowMultiple]
    internal sealed class SystemEngine : IGetReadyEngine
    {
        private readonly ISystem _system;
        private readonly ISimulation _simulation;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public SystemEngine(ISimulation simulation, ISystem system)
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
