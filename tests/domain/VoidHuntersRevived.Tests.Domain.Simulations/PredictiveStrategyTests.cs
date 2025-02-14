using VoidHuntersRevived.Tests.Common.Simulations.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Domain.Simulations
{
    public class PredictiveStrategyTests
    {
        public readonly SimulationMocker<LockstepStrategyMocker, PredictiveStrategyMocker> SimulationMocker;

        public PredictiveStrategyTests()
        {
            this.SimulationMocker = new SimulationMocker<LockstepStrategyMocker, PredictiveStrategyMocker>();
        }

        [Fact]
        public void FalsePrediction_IsReverted()
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(16);
        }
    }
}