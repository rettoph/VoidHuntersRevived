using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
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

        public SimulationMocker Input(VhId sourceId, IStepInput data)
        {
            foreach (IBaseStrategyMocker strategy in this.SimulationMockers)
            {
                strategy.Strategy.Events.Input(sourceId, data);
            }

            return this;
        }
    }
}

public class SimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : SimulationMocker
    where TLockstepStrategyMocker : LockstepStrategyMocker, new()
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
