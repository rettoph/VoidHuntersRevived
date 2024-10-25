using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;
using VoidHuntersRevived.Tests.Domain.Entities.Components;
using VoidHuntersRevived.Tests.Domain.Entities.Engines;
using VoidHuntersRevived.Tests.Domain.Entities.Events;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityService_SpawnDespawn_Tests : BaseSimulationTests<EntityService_SpawnDespawn_Tests>
    {
        public static readonly Key<IEntityTemplate> TestEntityTemplateKey = Key<IEntityTemplate>.GetByName("TestEntityTemplate");

        private readonly LockstepStrategy_Client _lockstep;
        private readonly PredictiveStrategy _predictive;


        public EntityService_SpawnDespawn_Tests() : base([typeof(ClientLockstepStrategyBuilder), typeof(PredictiveStrategyBuilder)])
        {
            _predictive = (PredictiveStrategy?)this.simulation[StrategyTypeEnum.Predictive] ?? throw new NotImplementedException();
            _lockstep = (LockstepStrategy_Client?)this.simulation[StrategyTypeEnum.Lockstep] ?? throw new NotImplementedException();
        }

        [Theory]
        [InlineData(100, 10, 16)]
        [InlineData(100, 10, 100)]
        [InlineData(1000, 50, 5)]
        public void EntityService_SpawnDespawn_PredictiveSynchronizesWithLockstep(int range, int segment, int simulatedRealtimeIntervalInMilliseconds)
        {
            Dictionary<IStrategy, int> totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(0, totals[_predictive]);
            Assert.Equal(0, totals[_lockstep]);

            // "Predict" 10 initial entities to be discarded
            this.InputMany(this.GenerateTestSpawnInput, segment, 0, false)
                .Update(simulatedRealtimeIntervalInMilliseconds, 4);

            // Ensure the prediction was made in the predictive strategy but not on the lockstep strategy
            totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(segment, totals[_predictive]);
            Assert.Equal(0, totals[_lockstep]);

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < segment; y++)
                {
                    bool verified = y == (segment / 2);

                    this.Update(simulatedRealtimeIntervalInMilliseconds, 1)
                        .Input(this.GenerateTestDepawnInput(y), verified)
                        .Input(this.GenerateTestSpawnInput(y + range), verified);
                }

                this.Update(simulatedRealtimeIntervalInMilliseconds, 10);
            }

            // Simulate long lapse of time - giving the predictive strategy time to catch up and revert as needed
            this.Update(simulatedRealtimeIntervalInMilliseconds, 1000);


            // Ensure an equal number of entities exist in both strategies
            // The code above needs much refatoring to become clear - essentially we spawn and despawn a small range of entities a huge amount of times
            // Every iteration we spawn 10 and despawn 10. Of the 20 spawn/despawn inputs - only 1 is actually validated on the lockstep simulation
            // This means 2 things:
            //  - There should only ever be 1 actual "real" "valid" entity within both simulations at any one time.
            //  - Most entities created on the predictive strategy will need to be reverted
            // This is all done in an effort to simulate spam clicking the tracktor beam to rapidly spawn/despawn pieces in game
            // Ideally the total number of entities within both simulations should be the same. A mismatch indicates some sort of desyncronization between 
            // the predictive and lockstep strategies
            totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(1, totals[_predictive]);
            Assert.Equal(1, totals[_lockstep]);
        }

        protected override IEnumerable<EntityTemplateFragment> GetEntityTemplateFragments()
        {
            yield return new EntityTemplateFragment()
            {
                Key = TestEntityTemplateKey,
                Components = [
                    new TestComponent()
                ]
            };
        }

        protected override IEnumerable<IEngine> GetEngines(IStrategyBuilder builder)
        {
            return [new TestInputEngine()];
        }

        private TestSpawnInput GenerateTestSpawnInput(int id)
        {
            return new TestSpawnInput() { EntityId = HashBuilder<TestSpawnInput, int>.Instance.Calculate(id), EntityTemplateKey = TestEntityTemplateKey };
        }

        private TestDepawnInput GenerateTestDepawnInput(int id)
        {
            return new TestDepawnInput() { EntityId = HashBuilder<TestDepawnInput, int>.Instance.Calculate(id) };
        }
    }
}