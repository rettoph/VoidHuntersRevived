using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
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