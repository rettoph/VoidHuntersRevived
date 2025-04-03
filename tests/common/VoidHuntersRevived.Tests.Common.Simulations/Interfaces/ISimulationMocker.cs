using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Tests.Common.Providers;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Simulations.Interfaces
{
    public interface ISimulationMocker
    {
        Simulation Simulation { get; }
        DefaultLockstepStrategyMocker DefaultLockstepStrategyMocker { get; }
        PredictiveStrategyMocker PredictiveStrategyMocker { get; }
    }

    public interface ISimulationMocker<out TSelf, TStrategyMocker> : ISimulationMocker
        where TSelf : ISimulationMocker
        where TStrategyMocker : IStrategyMocker
    {
        TSelf Update(TimeSpan interval, int count = 1);

        TSelf Input(VhId sourceId, IStepInput input, bool verified = true);
        TSelf Input<TStrategy>(VhId sourceId, IStepInput input, bool verified = true)
            where TStrategy : IStrategy;

        TSelf Input<TInput>(VhId sourceId, bool verified = true)
            where TInput : IStepInput, new();

        TSelf Input<TInput>(Func<int, TInput> factory, bool verified = true)
            where TInput : IStepInput;

        TSelf InputMany<TInput>(Func<int, TInput> factory, int count, bool verified = true)
            where TInput : IStepInput;

        TSelf Publish(VhId sourceId, IStepEvent @event, bool verified = false);

        TSelf Input(IStepEvent @event, bool verified = true);

        TSelf Publish<TEvent>(bool verified = true)
            where TEvent : IStepEvent, new();

        TSelf Invoke<TData>(Action<TData, VhIdProvider, TStrategyMocker> action, out MockData<TData> data)
            where TData : new();
        TSelf Invoke<TData>(MockData<TData> data, Action<TData, VhIdProvider, TStrategyMocker> action)
            where TData : new();
    }

    public interface ISimulationMocker<out TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker<TSelf, TStrategyMocker>
        where TSelf : ISimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IStrategyMocker
        where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : PredictiveStrategyMocker, TStrategyMocker, new()
    {
        TLockstepStrategyMocker LockstepStrategyMocker { get; }
        new TPredictiveStrategyMocker PredictiveStrategyMocker { get; }
    }
}
