using VoidHuntersRevived.Tests.Common.Entities.Extensions;
using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Domain.Entities.Mockers;
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

            this.SimulationMocker
                .Input(factory: TestSpawnEntityStepInput.Factory, verified: true)
                .Update(interval: simulatedRealtimeInterval);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 1);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval, count: 1000);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 1);
        }

        [Fact]
        public void UnverifiedEntitySpawn_IsReverted()
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(100);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);

            this.SimulationMocker
                .Input(factory: TestSpawnEntityStepInput.Factory, verified: false)
                .Update(interval: simulatedRealtimeInterval);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(
                lockstepExpected: 0,
                predictiveExpceted: 1);

            this.SimulationMocker.Update(interval: simulatedRealtimeInterval, count: 1000);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);
        }

        [Theory]
        [InlineData(100, 10, 16)]
        [InlineData(100, 10, 100)]
        [InlineData(1000, 50, 5)]
        public void RapidSpawnDespawn_PredictiveSynchronizesWithLockstep(int range, int segment, int simulatedRealtimeIntervalInMilliseconds)
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(simulatedRealtimeIntervalInMilliseconds);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 0);

            // "Predict" 10 initial entities to be discarded
            this.SimulationMocker
                .InputMany(TestSpawnEntityStepInput.Factory, segment, false)
                .Update(simulatedRealtimeInterval, 4);

            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(lockstepExpected: 0, predictiveExpceted: segment);

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < segment; y++)
                {
                    bool verified = y == (segment / 2);

                    this.SimulationMocker
                        .Update(simulatedRealtimeInterval, 1)
                        .Input(TestDepawnEntityStepInput.Factory(y), verified)
                        .Input(TestSpawnEntityStepInput.Factory(y + range), verified);
                }

                this.SimulationMocker.Update(simulatedRealtimeInterval, 10);
            }

            // Simulate long lapse of time - giving the predictive strategy time to catch up and revert as needed
            this.SimulationMocker.Update(simulatedRealtimeInterval, 1000);


            // Ensure an equal number of entities exist in both strategies
            // The code above needs much refatoring to become clear - essentially we spawn and despawn a small range of entities a huge amount of times
            // Every iteration we spawn 10 and despawn 10. Of the 20 spawn/despawn inputs - only 1 is actually validated on the lockstep simulation
            // This means 2 things:
            //  - There should only ever be 1 actual "real" "valid" entity within both simulations at any one time.
            //  - Most entities created on the predictive strategy will need to be reverted
            // This is all done in an effort to simulate spam clicking the tracktor beam to rapidly spawn/despawn pieces in game
            // Ideally the total number of entities within both simulations should be the same. A mismatch indicates some sort of desyncronization between 
            // the predictive and lockstep strategies
            this.SimulationMocker.AssertTotalEntities<TestEntityComponent>(expected: 1);
        }
    }
}
