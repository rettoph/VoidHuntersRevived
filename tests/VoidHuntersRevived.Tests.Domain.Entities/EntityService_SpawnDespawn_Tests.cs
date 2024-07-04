using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Extensions;
using VoidHuntersRevived.Tests.Domain.Entities.Components;
using VoidHuntersRevived.Tests.Domain.Entities.Descriptors;
using VoidHuntersRevived.Tests.Domain.Entities.Engines;
using VoidHuntersRevived.Tests.Domain.Entities.Events;
using VoidHuntersRevived.Tests.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityService_SpawnDespawn_Tests : IDisposable
    {
        public const string TestEntityTypeName = nameof(TestEntityTypeName);
        public static IEntityType TestEntityType = new EntityType<TestEntityDescriptor>(TestEntityTypeName);

        private int _sourceIdGeneratorIndex;
        private TickBuffer _tickBuffer;
        private ISimulation _simulation;
        private GameTime _gameTime;
        private List<EventDto> _inputs;
        private LockstepStrategy_Client _lockstep;
        private PredictiveStrategy _predictive;

        public EntityService_SpawnDespawn_Tests()
        {
            _sourceIdGeneratorIndex = 0;
            _inputs = new List<EventDto>();
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            _tickBuffer = new TickBuffer();
            _simulation = SimulationFactory.Build(VhId.Empty, PeerType.Client, [StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep], new()
            {
                { StrategyTypeEnum.Predictive, ctx => EntityEnginesFactory(scope: ctx, customEntityTypes: TestEntityType.Yield()) },
                { StrategyTypeEnum.Lockstep, ctx => EntityEnginesFactory(scope: ctx, customEntityTypes: TestEntityType.Yield()) }
            }, _tickBuffer);

            _lockstep = ((LockstepStrategy_Client?)_simulation.First(StrategyTypeEnum.Lockstep)) ?? throw new NotImplementedException();
            _predictive = ((PredictiveStrategy?)_simulation.First(StrategyTypeEnum.Predictive)) ?? throw new NotImplementedException();
        }

        public void Dispose()
        {
            _simulation.Dispose();
        }

        [Theory]
        [InlineData(100, 10, 16)]
        [InlineData(100, 10, 100)]
        [InlineData(1000, 50, 5)]
        public void EntityService_SpawnDespawn_PredictiveSynchronizesWithLockstep(int range, int segment, int simulatedRealtimeIntervalInMilliseconds)
        {
            Dictionary<StrategyTypeEnum, int> totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(0, totals[StrategyTypeEnum.Predictive]);
            Assert.Equal(0, totals[StrategyTypeEnum.Lockstep]);

            // "Predict" 10 initial entities to be discarded
            this.InputMany(this.GenerateTestSpawnInput, true, segment, 0).Update(simulatedRealtimeIntervalInMilliseconds, 4);

            // Ensure the prediction was made in the predictive strategy but not on the lockstep strategy
            totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(segment, totals[StrategyTypeEnum.Predictive]);
            Assert.Equal(0, totals[StrategyTypeEnum.Lockstep]);

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < segment; y++)
                {
                    bool doDiscard = y != (segment / 2);

                    this.Update(simulatedRealtimeIntervalInMilliseconds, 1)
                        .Input(this.GenerateTestDepawnInput(y, doDiscard))
                        .Input(this.GenerateTestSpawnInput(y + range, doDiscard));
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
            // This is all done in an effort to simulate smap clicking the tracktor beam to radically spawn/despawn pieces in game
            // Ideally the total number of entities within both simulations should be the same. A mismatch indicates some sort of desyncronization between 
            // the predictive and lockstep strategies
            totals = this.CalculateTotalEntities<TestComponent>();
            Assert.Equal(1, totals[StrategyTypeEnum.Predictive]);
            Assert.Equal(1, totals[StrategyTypeEnum.Lockstep]);
        }

        private EntityService_SpawnDespawn_Tests InputMany<T>(Func<int, bool, T> inputGenerator, bool doDiscard, int count, int offset)
            where T : TestInput, IInputData
        {
            for (int i = 0; i < count; i++)
            {
                this.Input(inputGenerator(i + offset, doDiscard));
            }

            return this;
        }

        private EntityService_SpawnDespawn_Tests Input<T>(T input)
            where T : TestInput, IInputData
        {
            VhId sourceId = this.GenerateSourceId();

            _simulation.Input(sourceId, input);

            if (input.DoDiscard == true)
            { // Simulate the "discarding" of a lockstep event - as if the server regected the event.
                return this;
            }

            _inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = input
            });

            return this;
        }

        private EntityService_SpawnDespawn_Tests Update(int interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_lockstep.StepsSinceTick == _lockstep.StepsPerTick)
                {
                    _tickBuffer.TryEnqueue(Tick.Create(_lockstep.CurrentTick.Id + 1, _inputs.ToArray()));
                    _inputs.Clear();
                }

                _simulation.Update(_gameTime.Step(interval));
            }

            return this;
        }

        private static IEnumerable<IEngine> EntityEnginesFactory(
            ILifetimeScope scope,
            IEnumerable<ComponentSerializer>? customComponentSerializers = null,
            IEnumerable<IEntityInitializer>? customEntityInitializers = null,
            IEnumerable<IEntityType>? customEntityTypes = null)
        {
            IFiltered<ComponentSerializer> componentSerializers = new MockFiltered<ComponentSerializer>(customComponentSerializers ?? Enumerable.Empty<ComponentSerializer>());
            IFiltered<IEntityInitializer> entityInitializers = new MockFiltered<IEntityInitializer>(customEntityInitializers ?? Enumerable.Empty<IEntityInitializer>());
            IFiltered<IEntityType> entityTypes = new MockFiltered<IEntityType>(customEntityTypes ?? Enumerable.Empty<IEntityType>());

            ILogger logger = scope.Resolve<ILogger>();
            EnginesRoot enginesRoot = scope.Resolve<EnginesRoot>();
            EntitiesSubmissionScheduler scheduler = scope.Resolve<EntitiesSubmissionScheduler>();

            IComponentSerializerService componentSerializerService = new ComponentSerializerService(componentSerializers);

            EntityTypeService entityTypeService = new EntityTypeService(entityInitializers, entityTypes, componentSerializerService.ToLazy(), enginesRoot);
            EntityService entityService = new EntityService(logger, ((IEntityTypeService)entityTypeService).ToLazy(), scheduler);

            yield return entityTypeService;
            yield return entityService;
            yield return new EntitySubmissionEngine(entityService);
            yield return new TestInputEngine();
        }

        private TestSpawnInput GenerateTestSpawnInput(int id, bool doDiscard)
        {
            return new TestSpawnInput() { EntityId = HashBuilder<TestEntityDescriptor, int>.Instance.Calculate(id), EntityType = TestEntityType, DoDiscard = doDiscard };
        }

        private TestDepawnInput GenerateTestDepawnInput(int id, bool doDiscard)
        {
            return new TestDepawnInput() { EntityId = HashBuilder<TestEntityDescriptor, int>.Instance.Calculate(id), DoDiscard = doDiscard };
        }

        private VhId GenerateSourceId()
        {
            return HashBuilder<SpawnEntity, int>.Instance.Calculate(_sourceIdGeneratorIndex++);
        }

        private Dictionary<StrategyTypeEnum, int> CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return _simulation.Strategies.ToDictionary(x => x.Type, x => x.Engines.Get<IEntityService>().CalculateTotal<T>());
        }
    }
}