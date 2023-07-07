using Guppy.Attributes;
using Guppy.Enums;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    [Service<IEngine>(ServiceLifetime.Scoped, true)]
    public abstract class BasicEngine<TSimulation> : ISimulationEngine<TSimulation>, IQueryingEntitiesEngine
        where TSimulation : ISimulation
    {
        public TSimulation Simulation { get; private set; } = default!;
        public EntitiesDB entitiesDB { get; set; } = null!;

        public BasicEngine()
        {
        }

        public virtual void Ready()
        {
        }

        public virtual void Initialize(TSimulation simulation)
        {
            this.Simulation = simulation;
        }
    }

    public abstract class BasicEngine : BasicEngine<ISimulation>
    {

    }
}
