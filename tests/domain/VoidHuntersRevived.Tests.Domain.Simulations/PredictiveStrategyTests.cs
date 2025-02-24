using Moq;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;
using VoidHuntersRevived.Tests.Common.Simulations.Stubs;
using VoidHuntersRevived.Tests.Domain.Simulations.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Simulations
{
    public class PredictiveStrategyTests
    {
        public readonly SimulationMocker<DefaultLockstepStrategyMocker, PredictiveStrategyMocker> SimulationMocker;

        public PredictiveStrategyTests()
        {
            this.SimulationMocker = new SimulationMocker<DefaultLockstepStrategyMocker, PredictiveStrategyMocker>();
        }

        [Fact]
        public void UnverifiedPrediction_IsReverted()
        {
            this.SimulationMocker.PublishThenUpdateThenVerifyPrediction<TestPublicPredictableStepEvent>(
                verified: false,
                publishTimes: Times.Once,
                revertTimes: Times.Once);
        }

        [Fact]
        public void VerifiedPrediction_IsNotReverted()
        {
            this.SimulationMocker.PublishThenUpdateThenVerifyPrediction<TestPublicPredictableStepEvent>(
                verified: true,
                publishTimes: Times.Once,
                revertTimes: Times.Never);
        }
    }
}