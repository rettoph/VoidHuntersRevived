using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateProviderService : StrategyEngine, IEntityTemplateProviderService, IQueryingEntitiesEngine, IOnInitializeEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTemplateService _entityTemplateService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;

        private readonly DoubleDictionary<IEntityTemplate, Key<IEntityTemplate>, IEntityTemplateProvider> _providers;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTemplateProviderService(
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
            _providers = _entityTemplateService.GetAll()
                .ToDoubleDictionary(
                    keySelector1: type => type,
                    keySelector2: type => type.Key,
                    elementSelector: type =>
                    {
                        return (IEntityTemplateProvider)new EntityTemplateProvider(
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
            foreach (IEntityTemplateProvider entityTemplateProvider in _providers.Values)
            {
                entityTemplateProvider.Initialize(
                    entitiesDB: this.entitiesDB,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: _componentSerializerService.Value);
            }
        }

        public IEntityTemplateProvider GetByKey(Key<IEntityTemplate> key)
        {
            return _providers[key];
        }

        public IEntityTemplateProvider GetByType(IEntityTemplate entityTemplate)
        {
            return _providers[entityTemplate];
        }
    }
}
