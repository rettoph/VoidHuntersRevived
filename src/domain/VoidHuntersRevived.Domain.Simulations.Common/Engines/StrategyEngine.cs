using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    [StrategyFilter<IStrategy>]
    [Service(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute | ServiceRegistrationFlags.AsImplementedInterfaces)]
    public abstract class StrategyEngine<TStrategy> : IEngine
        where TStrategy : IStrategy
    {
        public TStrategy Strategy { get; private set; } = default!;

        public StrategyEngine()
        {
        }

        public virtual void Ready()
        {
        }

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Begin)]
        public void Initialize(TStrategy strategy)
        {
            this.Strategy = strategy;
        }
    }

    public abstract class StrategyEngine : StrategyEngine<IStrategy>
    {

    }
}
