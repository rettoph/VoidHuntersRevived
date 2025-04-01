using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public abstract class SimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : ISimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : SimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : IStrategyMocker
        where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : PredictiveStrategyMocker, TStrategyMocker, new()
    {
        private readonly UniqueNumberProvider _uniqueNumbers = new();

        public TLockstepStrategyMocker LockstepStrategyMocker { get; }
        public TPredictiveStrategyMocker PredictiveStrategyMocker { get; }

        public Simulation Simulation { get; }

        DefaultLockstepStrategyMocker ISimulationMocker.DefaultLockstepStrategyMocker => this.LockstepStrategyMocker;

        PredictiveStrategyMocker ISimulationMocker.PredictiveStrategyMocker => this.PredictiveStrategyMocker;

        public SimulationMocker(VhId id = default)
        {
            this.PredictiveStrategyMocker = new TPredictiveStrategyMocker();
            this.LockstepStrategyMocker = new TLockstepStrategyMocker();

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

        public TSelf Update(TimeSpan interval, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                this.PredictiveStrategyMocker.Update(interval);
                this.LockstepStrategyMocker.Update(interval);
            }

            return (TSelf)this;
        }

        public TSelf Input(VhId sourceId, IStepInput input, bool verified = true)
        {
            if (verified == true)
            {
                this.LockstepStrategyMocker.Input(sourceId, input);
            }

            this.PredictiveStrategyMocker.Input(sourceId, input);

            return (TSelf)this;
        }

        public TSelf Input<TInput>(VhId sourceId, bool verified = true)
            where TInput : IStepInput, new()
        {
            return this.Input(sourceId, new TInput(), verified);
        }

        public TSelf Input<TInput>(Func<int, TInput> factory, bool verified = true)
            where TInput : IStepInput
        {
            int id = this._uniqueNumbers.GetInt32();
            VhId sourceId = HashBuilder<UniqueNumberProvider, int>.Instance.Calculate(id);
            return this.Input(sourceId, factory(id), verified);
        }

        public TSelf InputMany<TInput>(Func<int, TInput> factory, int count, bool verified = true)
            where TInput : IStepInput
        {
            for (int i = 0; i < count; i++)
            {
                int id = this._uniqueNumbers.GetInt32();
                VhId sourceId = HashBuilder<UniqueNumberProvider, int>.Instance.Calculate(id);
                this.Input(sourceId, factory(id), verified);
            }

            return (TSelf)this;
        }

        public TSelf Publish(VhId sourceId, IStepEvent @event, bool verified = false)
        {
            if (verified == true)
            {
                this.LockstepStrategyMocker.Publish(sourceId, @event);
            }

            this.PredictiveStrategyMocker.Publish(sourceId, @event);

            return (TSelf)this;
        }

        public TSelf Input(IStepEvent @event, bool verified = true)
        {
            return this.Publish(VhId.NewVhId(), @event, verified);
        }

        public TSelf Publish<TEvent>(bool verified = true)
            where TEvent : IStepEvent, new()
        {
            return this.Publish(VhId.NewVhId(), new TEvent(), verified);
        }
    }

    public sealed class SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker<SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, IStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : DefaultLockstepStrategyMocker, IStrategyMocker, new()
        where TPredictiveStrategyMocker : PredictiveStrategyMocker, IStrategyMocker, new()
    {
    }
}
