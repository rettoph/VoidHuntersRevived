using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group00)]
    public class EntityTypeProviderService : StrategyEngine, IEntityTypeProviderService, IQueryingEntitiesEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTypeService _entityTypeService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;
        private readonly IFiltered<IEntityTypeProviderInitializer> _entityTypeProviderInitializers;

        private Dictionary<IKey<IEntityType>, IEntityTypeProvider> _providers;
        private HashSet<Type> _distinctComponentTypes;
        private Dictionary<IKey<IEntityType>, IEntityTypeProvider[]> _implementationsById;
        private Dictionary<Type, IEntityTypeProvider[]> _implementationsByType;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTypeProviderService(
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityTypeService entityTypeService,
            IFiltered<IEntityTypeProviderInitializer> entityTypeProviderInitializers,
            Lazy<IComponentSerializerService> componentSerializerService,
            EnginesRoot enginesRoot,
            EntityService entityService)
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _entityTypeService = entityTypeService;
            _componentSerializerService = componentSerializerService;
            _entityTypeProviderInitializers = entityTypeProviderInitializers;
            _distinctComponentTypes = new HashSet<Type>();
            _implementationsById = new Dictionary<IKey<IEntityType>, IEntityTypeProvider[]>();
            _implementationsByType = new Dictionary<Type, IEntityTypeProvider[]>();

            // Create EntityTypeProviders for all registered IEntityType instances
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = enginesRoot.GenerateEntityFunctions();
            _providers = _entityTypeService.GetAll()
                .Where(x => x.Flags.HasFlag(EntityTypeFlags.Partial) == false)
                .ToDictionary(
                    keySelector: type => type.Key,
                    elementSelector: type =>
                    {
                        return (IEntityTypeProvider)new EntityTypeProvider(
                            type,
                            _entityTypeService,
                            _uniqueNumberProvider,
                            factory,
                            functions,
                            entityTypeProviderInitializers,
                            entityService
                        );
                    });
        }

        public override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);

            foreach (IEntityTypeProvider entityTypeProvider in _providers.Values)
            {
                entityTypeProvider.Initialize(
                    entitiesDB: this.entitiesDB,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: _componentSerializerService.Value,
                    entityTypeProviderInitializers: _entityTypeProviderInitializers);
            }
        }

        public IEntityTypeProvider GetByKey(IKey<IEntityType> key)
        {
            return _providers[key];
        }

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            if (_distinctComponentTypes.Count != 0)
            {
                return _distinctComponentTypes;
            }

            _distinctComponentTypes = _providers.Values.SelectMany(x => x.GetAllDistinctComponentTypes()).Distinct().ToHashSet();

            return _distinctComponentTypes;
        }

        public IEntityTypeProvider[] GetAllByKey(IKey<IEntityType> key)
        {
            ref IEntityTypeProvider[]? providers = ref CollectionsMarshal.GetValueRefOrAddDefault(_implementationsById, key, out bool exists);

            if (exists)
            {
                return providers!;
            }

            providers = _providers.Values.Where(x => x.Implements(key)).ToArray();
            return providers;
        }

        public IEntityTypeProvider[] GetAllByType<T>()
            where T : IEntityType
        {
            ref IEntityTypeProvider[]? providers = ref CollectionsMarshal.GetValueRefOrAddDefault(_implementationsByType, typeof(T), out bool exists);

            if (exists)
            {
                return providers!;
            }

            providers = _providers.Values.Where(x => x.Type.Key is IKey<T>).ToArray();
            return providers;
        }
    }
}
