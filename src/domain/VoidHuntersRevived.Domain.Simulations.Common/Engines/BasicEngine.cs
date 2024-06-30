using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    [Service(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute | ServiceRegistrationFlags.AsImplementedInterfaces)]
    public abstract class BasicEngine<TSimulation> : IStrategyEngine<TSimulation>
        where TSimulation : IStrategy
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

    public abstract class BasicEngine : BasicEngine<IStrategy>
    {

    }
}
