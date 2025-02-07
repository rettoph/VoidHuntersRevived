using Guppy.Game.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public abstract class StrategySystem<TStrategy> : ISceneSystem
        where TStrategy : IStrategy
    {
    }

    public abstract class StrategySystem : StrategySystem<IStrategy>
    {

    }
}