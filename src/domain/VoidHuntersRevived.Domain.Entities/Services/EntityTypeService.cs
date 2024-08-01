using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Guppy.Core.Resources.Common.Services;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
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
    public class EntityTypeService : StrategyEngine, IEntityTypeService, IQueryingEntitiesEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IFiltered<IEntityInitializer> _initializers;
        private readonly Lazy<IComponentSerializerService> _serializers;
        private readonly EnginesRoot _enginesRoot;

        private DoubleDictionary<Id<IEntityType>, IEntityType, IEntityTypeProvider> _providers;
        private Dictionary<Id<IEntityType>, IEntityType> _types;
        private Dictionary<Type, object> _byDescriptor;
        private HashSet<Type> _distinctComponentTypes;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTypeService(
            IFiltered<IEntityInitializer> initializers,
            IUniqueNumberProvider uniqueNumberProvider,
            IResourceService resources,
            Lazy<IComponentSerializerService> serializers,
            EnginesRoot enginesRoot)
        {
            _initializers = initializers;
            _uniqueNumberProvider = uniqueNumberProvider;
            _serializers = serializers;
            _enginesRoot = enginesRoot;
            _providers = null!;

            // Load all IEntityType instances for which there is an initializer (or has been imported as a resource)
            IEnumerable<IEntityType> resourceEntityTypes = resources.GetValues<IEntityType>().Select(x => x.Value);
            _types = initializers.SelectMany(init => init.ExplicitEntityTypes)
                .Concat(resourceEntityTypes)
                .Distinct()
                .Where(x => x.Flags.HasFlag(EntityTypeFlags.Partial) == false)
                .ToDictionary(x => x.Id, x => x);

            _byDescriptor = new Dictionary<Type, object>();
            _distinctComponentTypes = new HashSet<Type>();
        }

        public override void Initialize(IStrategy simulation)
        {
            base.Initialize(simulation);

            IEntityFactory factory = _enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = _enginesRoot.GenerateEntityFunctions();
            EntityService entities = this.Strategy.Engines.Get<EntityService>();


            // Create EntityTypeProviders for all registered IEntityType instances
            _providers = _types.Values
                .ToDoubleDictionary(
                    keySelector1: type => type.Id,
                    keySelector2: type => type,
                    elementSelector: type =>
                    {
                        return (IEntityTypeProvider)new EntityTypeProvider(
                            type,
                            _initializers.Where(init => init.ShouldInitialize(type)),
                            _uniqueNumberProvider,
                            factory,
                            functions,
                            this.Strategy.Engines,
                            _serializers.Value,
                            this.entitiesDB
                        );
                    });
        }

        public IEntityType GetById(Id<IEntityType> id)
        {
            return _providers[id].Type;
        }

        public IEnumerable<IEntityType> GetAll()
        {
            return _types.Values;
        }

        public IEntityType<T>[] GetAll<T>() where T : VoidHuntersEntityDescriptor
        {
            ref object? array = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(T), out bool exists);
            if (exists)
            {
                return (IEntityType<T>[])array!;
            }

            array = _types.Values.OfType<IEntityType<T>>().ToArray();
            return (IEntityType<T>[])array;
        }

        public bool TryGetByKey(string key, [MaybeNullWhen(false)] out IEntityType type)
        {
            Id<IEntityType> id = Id<IEntityType>.FromString(key);
            return _types.TryGetValue(id, out type);
        }

        public IEntityTypeProvider GetProviderByType(IEntityType type)
        {
            return _providers[type];
        }

        public IEntityTypeProvider GetProviderByTypeId(Id<IEntityType> id)
        {
            return _providers[id];
        }

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            if (_distinctComponentTypes.Count != 0)
            {
                return _distinctComponentTypes;
            }

            foreach (IEntityType type in _types.Values)
            {
                foreach (IComponentBuilder builder in type.Descriptor.Instance.componentsToBuild)
                {
                    _distinctComponentTypes.Add(builder.GetEntityComponentType());
                }

                foreach (IComponentBuilder builder in type.Descriptor.Type.componentsToBuild)
                {
                    _distinctComponentTypes.Add(builder.GetEntityComponentType());
                }
            }

            return _distinctComponentTypes;
        }
    }
}
