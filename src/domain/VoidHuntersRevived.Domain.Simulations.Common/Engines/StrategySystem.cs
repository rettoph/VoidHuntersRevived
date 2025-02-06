using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public abstract class StrategySystem<TStrategy> : ISceneSystem, IEngine
        where TStrategy : IStrategy
    {
        public TStrategy Strategy { get; private set; } = default!;

        public StrategySystem()
        {
        }

        public virtual void Ready()
        {
        }

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Begin)]
        public void Initialize(TStrategy strategy)
        {
            this.Strategy = strategy;
        }
    }

    public abstract class StrategySystem : StrategySystem<IStrategy>
    {

    }
}