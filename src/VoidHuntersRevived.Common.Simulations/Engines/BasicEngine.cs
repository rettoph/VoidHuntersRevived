using Guppy.Attributes;
using Guppy.Enums;
using Svelto.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Attributes;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    [Service<IEngine>(ServiceLifetime.Scoped, true)]
    public abstract class BasicEngine<TSimulation> : ISimulationEngine<TSimulation>
        where TSimulation : ISimulation
    {
        public TSimulation Simulation { get; private set; } = default!;

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
