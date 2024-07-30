using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    [StrategyFilter<IStrategy>]
    [Service(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute | ServiceRegistrationFlags.AsImplementedInterfaces)]
    public abstract class StrategyEngine<TStrategy> : IStrategyEngine<TStrategy>
        where TStrategy : IStrategy
    {
        public TStrategy Strategy { get; private set; } = default!;

        public StrategyEngine()
        {
        }

        public virtual void Ready()
        {
        }

        public virtual void Initialize(TStrategy strategy)
        {
            this.Strategy = strategy;
        }
    }

    public abstract class StrategyEngine : StrategyEngine<IStrategy>
    {

    }
}
