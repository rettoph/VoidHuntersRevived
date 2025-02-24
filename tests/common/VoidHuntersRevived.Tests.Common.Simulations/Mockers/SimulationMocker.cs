using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public abstract class SimulationMocker : ISimulationMocker
    {
        private readonly UniqueNumberProvider _uniqueNumbers = new();

        public abstract Simulation Simulation { get; }

        public abstract IStrategyMocker<Strategy>[] SimulationMockers { get; }

        public void Update(TimeSpan interval, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (IStrategyMocker strategy in this.SimulationMockers)
                {
                    strategy.Update(interval, 1);
                }
            }
        }

        public void Input(VhId sourceId, IStepInput input, bool verified = true)
        {
            if (verified == false)
            {
                foreach (IStrategyMocker<PredictiveStrategy> strategy in this.SimulationMockers.OfType<IStrategyMocker<PredictiveStrategy>>())
                {
                    strategy.Object.Events.Input(sourceId, input);
                }

                return;
            }

            foreach (IStrategyMocker<Strategy> strategy in this.SimulationMockers)
            {
                strategy.Object.Events.Input(sourceId, input);
            }
        }

        public void Input<TInput>(VhId sourceId, bool verified = true)
            where TInput : IStepInput, new()
        {
            this.Input(sourceId, new TInput(), verified);
        }

        public void Input<TInput>(Func<int, TInput> factory, bool verified = true)
            where TInput : IStepInput
        {
            int id = this._uniqueNumbers.GetInt32();
            VhId sourceId = HashBuilder<UniqueNumberProvider, int>.Instance.Calculate(id);
            this.Input(sourceId, factory(id), verified);
        }

        public void InputMany<TInput>(Func<int, TInput> factory, int count, bool verified = true)
            where TInput : IStepInput
        {
            for (int i = 0; i < count; i++)
            {
                int id = this._uniqueNumbers.GetInt32();
                VhId sourceId = HashBuilder<UniqueNumberProvider, int>.Instance.Calculate(id);
                this.Input(sourceId, factory(id), verified);
            }
        }

        public void Publish(VhId sourceId, IStepEvent @event, bool verified = false)
        {
            if (verified == false)
            {
                foreach (IStrategyMocker<PredictiveStrategy> strategy in this.SimulationMockers.OfType<IStrategyMocker<PredictiveStrategy>>())
                {
                    strategy.Object.Events.Publish(sourceId, @event);
                }

                return;
            }

            foreach (IStrategyMocker<Strategy> strategy in this.SimulationMockers)
            {
                strategy.Object.Events.Publish(sourceId, @event);
            }
        }

        public void Input(IStepEvent @event, bool verified = true)
        {
            this.Publish(VhId.NewVhId(), @event, verified);
        }

        public void Publish<TEvent>(bool verified = true)
            where TEvent : IStepEvent, new()
        {
            this.Publish(VhId.NewVhId(), new TEvent(), verified);
        }
    }
}

public class SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker, ISimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>
    where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, new()
    where TPredictiveStrategyMocker : PredictiveStrategyMocker, new()
{
    public TLockstepStrategyMocker LockstepStrategyMocker { get; }
    public TPredictiveStrategyMocker PredictiveStrategyMocker { get; }

    public override Simulation Simulation { get; }

    public override IStrategyMocker<Strategy>[] SimulationMockers { get; }

    public SimulationMocker(VhId id = default)
    {
        this.PredictiveStrategyMocker = new TPredictiveStrategyMocker();
        this.LockstepStrategyMocker = new TLockstepStrategyMocker();
        this.SimulationMockers = [
            this.LockstepStrategyMocker,
            this.PredictiveStrategyMocker
        ];

        this.Simulation = new Simulation(id, this.StrategyFactory);
        this.Simulation.Initialize();
    }

    private IEnumerable<IStrategy> StrategyFactory(ISimulation simulation)
    {
        this.LockstepStrategyMocker.Simulation = simulation;
        this.PredictiveStrategyMocker.Simulation = simulation;

        return [
            this.LockstepStrategyMocker.Strategy,
            this.PredictiveStrategyMocker.Strategy
        ];
    }
}
