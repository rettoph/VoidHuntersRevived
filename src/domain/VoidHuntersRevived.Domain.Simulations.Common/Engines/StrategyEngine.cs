using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    [StrategyFilter<IStrategy>]
    [Service(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute | ServiceRegistrationFlags.AsImplementedInterfaces)]
    public abstract class StrategyEngine<TSimulation> : IStrategyEngine<TSimulation>
        where TSimulation : IStrategy
    {
        public TSimulation Simulation { get; private set; } = default!;

        public StrategyEngine()
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

    public abstract class StrategyEngine : StrategyEngine<IStrategy>
    {

    }
}
