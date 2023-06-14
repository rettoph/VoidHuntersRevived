using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    [Service<IEngine>(ServiceLifetime.Transient, true)]
    public abstract class BasicEngine : ISimulationEngine, IQueryingEntitiesEngine
    {
        public ISimulation Simulation { get; private set; } = null!;
        public EntitiesDB entitiesDB { get; set; } = null!;

        public virtual void Ready()
        {
        }

        public virtual void Initialize(ISimulation simulation)
        {
            this.Simulation = simulation;
        }
    }
}
