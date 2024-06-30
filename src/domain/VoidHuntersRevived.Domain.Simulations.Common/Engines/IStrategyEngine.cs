using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IStrategyEngine<in TStrategy> : IEngine
        where TStrategy : IStrategy
    {
        void Initialize(TStrategy strategy);
    }

    public interface IStrategyEngine : IStrategyEngine<IStrategy>
    {

    }
}
