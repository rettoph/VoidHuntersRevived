using Moq;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using VoidHuntersRevived.Tests.Common.Simulations.Stubs;
using VoidHuntersRevived.Tests.Domain.Simulations.Extensions;

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
            this.SimulationMocker.PublishThenUpdateThenVerifyPrediction<TestPublicPredictableStepEvent>(
                onlyPrediction: true,
                publishTimes: Times.Once,
                revertTimes: Times.Once);
        }

        [Fact]
        public void TruePrediction_IsNotReverted()
        {
            this.SimulationMocker.PublishThenUpdateThenVerifyPrediction<TestPublicPredictableStepEvent>(
                onlyPrediction: false,
                publishTimes: Times.Once,
                revertTimes: Times.Never);
        }
    }
}