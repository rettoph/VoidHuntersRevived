using Guppy.Core.Common.Extensions;
using Guppy.Core.Resources.Common;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Tests.Common.Entities.Stubs;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;
using VoidHuntersRevived.Tests.Domain.Entities.Systems;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityService_SpawnDespawn_Tests
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private readonly SimulationMock _simulation;
        private readonly IStrategyMocker<LockstepStrategy> _lockstep;
        private readonly IStrategyMocker<PredictiveStrategy> _predictive;

        public EntityService_SpawnDespawn_Tests() : base()
        {
            this._simulation = new SimulationMockBuilder(
                    id: VhId.Empty,
                    stepInterval: this.StepInterval,
                    stepsPerTick: this.StepsPerTick,
                    entityTemplateFragments: [TestEntityComponent.TestEntityTemplateFragment]
                )
                .AddStrategy<PredictiveStrategy>()
                .AddStrategy<LockstepStrategy>()
                .Register(builder =>
                {
                    builder.RegisterInstance(Enumerable.Empty<IComponentSerializer>());

                    builder.RegisterSceneFilter<IStrategy>(builder =>
                    {
                        builder.RegisterSceneSystem<TestInputSystem>();
                    });
                })
                .Build();

            this._predictive = this._simulation.Get<PredictiveStrategy>();
            this._lockstep = this._simulation.Get<LockstepStrategy>();
        }

        [Theory]
        [InlineData(100, 10, 16)]
        [InlineData(100, 10, 100)]
        [InlineData(1000, 50, 5)]
        public void EntityService_SpawnDespawn_PredictiveSynchronizesWithLockstep(int range, int segment, int simulatedRealtimeIntervalInMilliseconds)
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(simulatedRealtimeIntervalInMilliseconds);

            Dictionary<IStrategyAutoMock, int> totals = this._simulation.CalculateTotalEntities<TestEntityComponent>();
            Assert.Equal(0, totals[this._predictive]);
            Assert.Equal(0, totals[this._lockstep]);

            // "Predict" 10 initial entities to be discarded
            this._simulation.InputMany(TestSpawnEntityStepInput.Factory, segment, 0, false)
                .Update(simulatedRealtimeInterval, 4);

            // Ensure the prediction was made in the predictive strategy but not on the lockstep strategy
            totals = this._simulation.CalculateTotalEntities<TestEntityComponent>();
            Assert.Equal(segment, totals[this._predictive]);
            Assert.Equal(0, totals[this._lockstep]);

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < segment; y++)
                {
                    bool verified = y == (segment / 2);

                    this._simulation.Update(simulatedRealtimeInterval, 1)
                        .Input(TestDepawnEntityStepInput.Factory(y), verified)
                        .Input(TestSpawnEntityStepInput.Factory(y + range), verified);
                }

                this._simulation.Update(simulatedRealtimeInterval, 10);
            }

            // Simulate long lapse of time - giving the predictive strategy time to catch up and revert as needed
            this._simulation.Update(simulatedRealtimeInterval, 1000);


            // Ensure an equal number of entities exist in both strategies
            // The code above needs much refatoring to become clear - essentially we spawn and despawn a small range of entities a huge amount of times
            // Every iteration we spawn 10 and despawn 10. Of the 20 spawn/despawn inputs - only 1 is actually validated on the lockstep simulation
            // This means 2 things:
            //  - There should only ever be 1 actual "real" "valid" entity within both simulations at any one time.
            //  - Most entities created on the predictive strategy will need to be reverted
            // This is all done in an effort to simulate spam clicking the tracktor beam to rapidly spawn/despawn pieces in game
            // Ideally the total number of entities within both simulations should be the same. A mismatch indicates some sort of desyncronization between 
            // the predictive and lockstep strategies
            totals = this._simulation.CalculateTotalEntities<TestEntityComponent>();
            Assert.Equal(1, totals[this._predictive]);
            Assert.Equal(1, totals[this._lockstep]);
        }
    }
}