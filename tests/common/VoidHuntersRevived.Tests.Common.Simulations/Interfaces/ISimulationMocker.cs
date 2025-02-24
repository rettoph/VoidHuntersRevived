using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Simulations.Interfaces
{
    public interface ISimulationMocker
    {
        Simulation Simulation { get; }
        IStrategyMocker<Strategy>[] SimulationMockers { get; }

        void Update(TimeSpan interval, int count = 1);

        void Input(VhId sourceId, IStepInput input, bool verified = true);

        void Input<TInput>(VhId sourceId, bool verified = true)
            where TInput : IStepInput, new();

        void Input<TInput>(Func<int, TInput> factory, bool verified = true)
            where TInput : IStepInput;

        void InputMany<TInput>(Func<int, TInput> factory, int count, bool verified = true)
            where TInput : IStepInput;

        void Publish(VhId sourceId, IStepEvent @event, bool verified = false);

        void Input(IStepEvent @event, bool verified = true);

        void Publish<TEvent>(bool verified = true)
            where TEvent : IStepEvent, new();
    }

    public interface ISimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker
        where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, new()
        where TPredictiveStrategyMocker : PredictiveStrategyMocker, new()
    {
        TLockstepStrategyMocker LockstepStrategyMocker { get; }
        TPredictiveStrategyMocker PredictiveStrategyMocker { get; }
    }
}
