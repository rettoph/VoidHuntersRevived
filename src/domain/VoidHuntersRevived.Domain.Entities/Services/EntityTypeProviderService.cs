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
    public class EntityTypeProviderService : StrategyEngine, IEntityTypeProviderService, IQueryingEntitiesEngine, IOnInitializeEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTypeService _entityTypeService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;

        private DoubleDictionary<IEntityType, Key<IEntityType>, IEntityTypeProvider> _providers;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTypeProviderService(
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityTypeService entityTypeService,
            Lazy<IComponentSerializerService> componentSerializerService,
            EnginesRoot enginesRoot,
            EntityService entityService)
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _entityTypeService = entityTypeService;
            _componentSerializerService = componentSerializerService;

            // Create EntityTypeProviders for all registered IEntityType instances
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = enginesRoot.GenerateEntityFunctions();
            _providers = _entityTypeService.GetAll()
                .ToDoubleDictionary(
                    keySelector1: type => type,
                    keySelector2: type => type.Key,
                    elementSelector: type =>
                    {
                        return (IEntityTypeProvider)new EntityTypeProvider(
                            type,
                            _entityTypeService,
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
            foreach (IEntityTypeProvider entityTypeProvider in _providers.Values)
            {
                entityTypeProvider.Initialize(
                    entitiesDB: this.entitiesDB,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: _componentSerializerService.Value);
            }
        }

        public IEntityTypeProvider GetByKey(Key<IEntityType> key)
        {
            return _providers[key];
        }

        public IEntityTypeProvider GetByType(IEntityType entityType)
        {
            return _providers[entityType];
        }
    }
}
