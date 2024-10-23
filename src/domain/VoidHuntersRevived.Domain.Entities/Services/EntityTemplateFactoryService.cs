using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Factories;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Factories;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateFactoryService : StrategyEngine, IEntityTemplateFactoryService, IQueryingEntitiesEngine, IOnInitializeEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTemplateService _entityTemplateService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;

        private readonly DoubleDictionary<IEntityTemplate, Key<IEntityTemplate>, IEntityTemplateFactory> _factories;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTemplateFactoryService(
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityTemplateService entityTemplateService,
            Lazy<IComponentSerializerService> componentSerializerService,
            EnginesRoot enginesRoot,
            EntityService entityService)
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _entityTemplateService = entityTemplateService;
            _componentSerializerService = componentSerializerService;

            // Create EntityTemplateProviders for all registered IEntityTemplate instances
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = enginesRoot.GenerateEntityFunctions();
            _factories = _entityTemplateService.GetAll()
                .ToDoubleDictionary(
                    keySelector1: type => type,
                    keySelector2: type => type.Key,
                    elementSelector: type =>
                    {
                        return (IEntityTemplateFactory)new EntityTemplateFactory(
                            type,
                            _entityTemplateService,
                            _uniqueNumberProvider,
                            factory,
                            functions,
                            entityService
                        );
                    });
        }

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.PreInitialize)]
        public void OnInitialize(IStrategy strategy)
        {
            foreach (IEntityTemplateFactory entityTemplateProvider in _factories.Values)
            {
                entityTemplateProvider.Initialize(
                    entitiesDB: this.entitiesDB,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: _componentSerializerService.Value);
            }
        }

        public IEntityTemplateFactory GetByKey(Key<IEntityTemplate> key)
        {
            return _factories[key];
        }

        public IEntityTemplateFactory GetByType(IEntityTemplate entityTemplate)
        {
            return _factories[entityTemplate];
        }
    }
}
