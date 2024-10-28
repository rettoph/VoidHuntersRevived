using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public interface IStrategyMocker
    {
        IStrategy Instance { get; }
        ServiceProviderMocker Services { get; }
    }

    public interface IStrategyMocker<out TStrategy> : IStrategyMocker
        where TStrategy : IStrategy
    {
        new TStrategy Instance { get; }
    }

    public class StrategyMocker<TStrategy>(
        TStrategy instance,
        ServiceProviderMocker services
    ) : IStrategyMocker<TStrategy>
        where TStrategy : IStrategy
    {
        public TStrategy Instance { get; } = instance;
        public ServiceProviderMocker Services { get; } = services;

        IStrategy IStrategyMocker.Instance => this.Instance;
    }
}
