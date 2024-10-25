using Guppy.Core.Resources.Common;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Entities.Services;
using VoidHuntersRevived.Tests.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public abstract class BaseSimulationTests<TSelf>
        where TSelf : BaseSimulationTests<TSelf>
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private int _sourceIdGeneratorIndex;
        private readonly TickBuffer _tickBuffer;
        private readonly ISimulation _simulation;
        private readonly List<EventDto> _inputs;
        private readonly GameTime _gameTime;
        private readonly ILockstepStrategy _lockstep;
        private readonly PredictiveStrategy[] _predictives;

        protected TickBuffer tickBuffer => _tickBuffer;
        protected ISimulation simulation => _simulation;

        public BaseSimulationTests(IEnumerable<Type> strategyBuilderTypes)
        {
            _sourceIdGeneratorIndex = 0;

            _inputs = [];
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            _tickBuffer = new TickBuffer();
            _simulation = new SimulationBuilder(
                id: VhId.Empty,
                strategiesBuilder: new StrategiesBuilder(
                    strategyBuilderTypes: strategyBuilderTypes,
                    configuration: this.ConfigureStrategy)).Build();

            _lockstep = _simulation.Strategies.OfType<ILockstepStrategy>().Single();
            _predictives = _simulation.Strategies.OfType<PredictiveStrategy>().ToArray();
        }

        public void Dispose()
        {
            _simulation.Dispose();
        }

        protected TSelf Input<T>(T input, bool verified)
            where T : IInputData
        {
            VhId sourceId = this.GenerateSourceId();

            foreach (PredictiveStrategy predictive in _predictives)
            {
                predictive.Input(sourceId, input);
            }

            if (verified == false)
            { // Simulate the "discarding" of a lockstep event - as if the server rejected the event.
                return (TSelf)this;
            }

            _inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = input
            });

            return (TSelf)this;
        }

        protected TSelf InputMany<T>(Func<int, T> inputGenerator, int count, int offset, bool verified)
            where T : IInputData
        {
            for (int i = 0; i < count; i++)
            {
                this.Input(inputGenerator(i + offset), verified);
            }

            return (TSelf)this;
        }

        protected TSelf Update(int interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_lockstep.StepsSinceTick == _lockstep.StepsPerTick)
                {
                    this.tickBuffer.TryEnqueue(Tick.Create(_lockstep.CurrentTick.Id + 1, _inputs.ToArray()));
                    _inputs.Clear();
                }

                this.simulation.Update(_gameTime.Step(interval));
            }

            return (TSelf)this;
        }

        protected virtual void ConfigureStrategy(IStrategyBuilder builder)
        {
            // Setup mocks
            EntitiesSubmissionScheduler entitiesSubmissionScheduler = new();
            EnginesRoot enginesRoot = new(entitiesSubmissionScheduler);

            EntityServiceBuilder entityService = new();
            entityService.EntityTemplateService.Setup(x => x.GetAll(), this.GetEntityTemplates(builder));
            entityService.EntityTypeProviderService.UniqueNumberProviderService.SetInstance(new UniqueNumberProvider());
            entityService.EntityTypeProviderService.EnginesRoot.SetInstance(enginesRoot);
            entityService.EntityQueryService.SetInstance(new EntityQueryService());
            entityService.EntitySpawnService.SetInstance(new EntitySpawnService(entityService.EntityQueryService.GetInstance(), entityService.EntityTypeProviderService.GetInstance(), entityService.GetInstance(), builder.Logger.GetInstance()));

            // Configure strategy
            builder.TickBuffer.SetInstance(_tickBuffer);
            builder.EngineServiceBuilder.EntitiesSubmissionScheduler.SetInstance(entitiesSubmissionScheduler);
            builder.EngineServiceBuilder.EnginesRoot.SetInstance(enginesRoot);
            builder.SettingService
                .Setup(settings => settings.GetValue(Settings.StepInterval), () => StepInterval)
                .Setup(settings => settings.GetValue(Settings.StepsPerTick), () => StepsPerTick);

            builder.EngineServiceBuilder.Engines.AddRange([
                entityService.EntityTypeProviderService.GetInstance(),
                entityService.GetInstance(),
                entityService.EntityQueryService.GetInstance(),
                entityService.EntitySpawnService.GetInstance(),
                new EntitySubmissionEngine(entitiesSubmissionScheduler),
            ]);

            builder.EngineServiceBuilder.Engines.AddRange(this.GetEngines(builder));
        }

        protected abstract IEnumerable<EntityTemplate> GetEntityTemplates(IStrategyBuilder builder);

        protected abstract IEnumerable<IEngine> GetEngines(IStrategyBuilder builder);

        protected virtual VhId GenerateSourceId()
        {
            return HashBuilder<SpawnEntity, int>.Instance.Calculate(_sourceIdGeneratorIndex++);
        }

        protected Dictionary<IStrategy, int> CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return _simulation.Strategies.ToDictionary(x => x, x => x.Engines.Get<IEntityQueryService>().CalculateTotal<T>());
        }
    }
}
