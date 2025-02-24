using VoidHuntersRevived.Tests.Common.Entities.Extensions;
using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Domain.Entities.Builders;
using VoidHuntersRevived.Tests.Entities.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityServiceTests
    {
        public readonly EntitySimulationMocker<TestEntityLockstepStrategyBuilder, TestEntityPredictiveStrategyBuilder> SimulationMocker;

        public EntityServiceTests()
        {
            this.SimulationMocker = new EntitySimulationMocker<TestEntityLockstepStrategyBuilder, TestEntityPredictiveStrategyBuilder>();
        }

        [Fact]
        public void VerifiedEntitySpawn_IsNotReverted()
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(100);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);

            this.SimulationMocker.Input(factory: TestSpawnEntityStepInput.Factory, verified: true);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(
                lockstepExpected: 0,
                predictiveExpceted: 1);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval, count: 1000);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);
        }

        [Fact]
        public void UnverifiedEntitySpawn_IsReverted()
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(100);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);

            this.SimulationMocker.Input(factory: TestSpawnEntityStepInput.Factory, verified: false);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(
                lockstepExpected: 0,
                predictiveExpceted: 1);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval, count: 1000);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);
        }
    }
}
