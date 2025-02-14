using Guppy.Core.Resources.Common;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Domain.Entities.Components;
using VoidHuntersRevived.Tests.Domain.Entities.Events;
using VoidHuntersRevived.Tests.Domain.Entities.Systems;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityService_SpawnDespawn_Tests
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        public static readonly Key<IEntityTemplate> TestEntityTemplateKey = Key<IEntityTemplate>.GetByName("TestEntityTemplate");

        private readonly SimulationMocker _simulation;
        private readonly IStrategyMocker<LockstepStrategy_Client> _lockstep;
        private readonly IStrategyMocker<PredictiveStrategy> _predictive;

        public EntityService_SpawnDespawn_Tests() : base()
        {
            this._simulation = new SimulationBuilder(
                    id: VhId.Empty,
                    stepInterval: this.StepInterval,
                    stepsPerTick: this.StepsPerTick,
                    entityTemplateFragments: [
                        new EntityTemplateFragment()
                        {
                            Key = TestEntityTemplateKey,
                            Components = [
                                new TestComponent()
                            ]
                        }
                    ]
                )
                .AddStrategy<PredictiveStrategy>()
                .AddStrategy<LockstepStrategy_Client>()
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
            this._lockstep = this._simulation.Get<LockstepStrategy_Client>();
        }

        [Theory]
        [InlineData(100, 10, 16)]
        [InlineData(100, 10, 100)]
        [InlineData(1000, 50, 5)]
        public void EntityService_SpawnDespawn_PredictiveSynchronizesWithLockstep(int range, int segment, int simulatedRealtimeIntervalInMilliseconds)
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(simulatedRealtimeIntervalInMilliseconds);

            Dictionary<IStrategyMocker, int> totals = this._simulation.CalculateTotalEntities<TestComponent>();
            Assert.Equal(0, totals[this._predictive]);
            Assert.Equal(0, totals[this._lockstep]);

            // "Predict" 10 initial entities to be discarded
            this._simulation.InputMany(this.GenerateTestSpawnInput, segment, 0, false)
                .Update(simulatedRealtimeInterval, 4);

            // Ensure the prediction was made in the predictive strategy but not on the lockstep strategy
            totals = this._simulation.CalculateTotalEntities<TestComponent>();
            Assert.Equal(segment, totals[this._predictive]);
            Assert.Equal(0, totals[this._lockstep]);

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < segment; y++)
                {
                    bool verified = y == (segment / 2);

                    this._simulation.Update(simulatedRealtimeInterval, 1)
                        .Input(GenerateTestDepawnInput(y), verified)
                        .Input(this.GenerateTestSpawnInput(y + range), verified);
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
            totals = this._simulation.CalculateTotalEntities<TestComponent>();
            Assert.Equal(1, totals[this._predictive]);
            Assert.Equal(1, totals[this._lockstep]);
        }

        private TestSpawnInput GenerateTestSpawnInput(int id)
        {
            return new() { EntityGlobalId = HashBuilder<TestSpawnInput, int>.Instance.Calculate(id).ToGlobalEntityId(), EntityTemplateKey = TestEntityTemplateKey };
        }

        private static TestDepawnInput GenerateTestDepawnInput(int id)
        {
            return new() { EntityGlobalId = HashBuilder<TestDepawnInput, int>.Instance.Calculate(id).ToGlobalEntityId() };
        }
    }
}