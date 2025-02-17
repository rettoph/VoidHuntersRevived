using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public abstract class SimulationMocker
    {
        public abstract IBaseStrategyMocker[] SimulationMockers { get; }

        public SimulationMocker Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (IBaseStrategyMocker strategy in this.SimulationMockers)
                {
                    strategy.Update(interval, 1);
                }
            }

            return this;
        }

        public SimulationMocker Input(VhId sourceId, IStepInput input, bool onlyPrediction = false)
        {
            if (onlyPrediction == true)
            {
                foreach (IBaseStrategyMocker strategy in this.SimulationMockers.OfType<IBaseStrategyMocker<PredictiveStrategy>>())
                {
                    strategy.Strategy.Events.Input(sourceId, input);
                }

                return this;
            }

            foreach (IBaseStrategyMocker strategy in this.SimulationMockers)
            {
                strategy.Strategy.Events.Input(sourceId, input);
            }

            return this;
        }

        public void Input(IStepInput input, bool onlyPrediction = false)
        {
            this.Input(VhId.NewVhId(), input, onlyPrediction);
        }

        public void Input<TInput>(bool onlyPrediction = false)
            where TInput : IStepInput, new()
        {
            this.Input(VhId.NewVhId(), new TInput(), onlyPrediction);
        }

        public SimulationMocker Publish(VhId sourceId, IStepEvent @event, bool onlyPrediction = false)
        {
            if (onlyPrediction == true)
            {
                foreach (IBaseStrategyMocker strategy in this.SimulationMockers.OfType<IBaseStrategyMocker<PredictiveStrategy>>())
                {
                    strategy.Strategy.Events.Publish(sourceId, @event);
                }

                return this;
            }

            foreach (IBaseStrategyMocker strategy in this.SimulationMockers)
            {
                strategy.Strategy.Events.Publish(sourceId, @event);
            }

            return this;
        }

        public void Input(IStepEvent @event, bool onlyPrediction = false)
        {
            this.Publish(VhId.NewVhId(), @event, onlyPrediction);
        }

        public void Publish<TEvent>(bool onlyPrediction = false)
            where TEvent : IStepEvent, new()
        {
            this.Publish(VhId.NewVhId(), new TEvent(), onlyPrediction);
        }
    }
}

public class SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker
    where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, new()
    where TPredictiveStrategyMocker : PredictiveStrategyMocker, new()
{
    public readonly TLockstepStrategyMocker LockstepStrategyMocker;
    public readonly TPredictiveStrategyMocker PredictiveStrategyMocker;

    public readonly Simulation Simulation;

    public override IBaseStrategyMocker[] SimulationMockers { get; }

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
        this.LockstepStrategyMocker.SimulationMocker.SetInstance(simulation);
        this.PredictiveStrategyMocker.SimulationMocker.SetInstance(simulation);

        return [
            this.LockstepStrategyMocker.Strategy,
            this.PredictiveStrategyMocker.Strategy
        ];
    }
}
