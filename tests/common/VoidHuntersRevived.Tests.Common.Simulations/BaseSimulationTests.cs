using Guppy.Core.Resources.Common;
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
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Tests.Common.Entities.Services;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public abstract class BaseSimulationTests
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private int _sourceIdGeneratorIndex;
        private readonly TickBuffer _tickBuffer;
        private readonly ISimulation _simulation;

        protected TickBuffer tickBuffer => _tickBuffer;
        protected ISimulation simulation => _simulation;
        protected abstract IEntityType[] EntityTypes { get; }

        public BaseSimulationTests(IEnumerable<Type> strategyBuilderTypes)
        {
            _sourceIdGeneratorIndex = 0;

            _tickBuffer = new TickBuffer();
            _simulation = new SimulationBuilder(
                id: VhId.Empty,
                strategiesBuilder: new StrategiesBuilder(
                    strategyBuilderTypes: strategyBuilderTypes,
                    configuration: this.ConfigureStrategy)).Build();
        }

        public void Dispose()
        {
            _simulation.Dispose();
        }

        protected virtual void ConfigureStrategy(IStrategyBuilder builder)
        {
            // Setup mocks
            EntitiesSubmissionScheduler entitiesSubmissionScheduler = new();
            EnginesRoot enginesRoot = new(entitiesSubmissionScheduler);

            EntityServiceBuilder entityService = new();
            entityService.EntityTypeService.Setup(x => x.GetAll(), this.EntityTypes);
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
        }

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
