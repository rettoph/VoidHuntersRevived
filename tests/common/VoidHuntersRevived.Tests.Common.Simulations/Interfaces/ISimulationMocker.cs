using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Simulations.Interfaces
{
    public interface ISimulationMocker
    {
        Simulation Simulation { get; }
        DefaultLockstepStrategyMocker DefaultLockstepStrategyMocker { get; }
        PredictiveStrategyMocker PredictiveStrategyMocker { get; }
    }

    public interface ISimulationMocker<out TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker
        where TSelf : ISimulationMocker<TSelf, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : PredictiveStrategyMocker, new()
    {
        TLockstepStrategyMocker LockstepStrategyMocker { get; }
        new TPredictiveStrategyMocker PredictiveStrategyMocker { get; }

        TSelf Update(TimeSpan interval, int count = 1);

        TSelf Input(VhId sourceId, IStepInput input, bool verified = true);

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
    }
}
