using Guppy.Core.Common;
using Guppy.Core.Common.Collections;
using Guppy.Core.Resources.Common;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeService : BasicEngine, IEntityTypeService, IQueryingEntitiesEngine, IReactOnAddEx<InstanceData>, IReactOnAddEx<TypeData>, IEngineEngine
    {
        private readonly IFiltered<IEntityInitializer> _initializers;
        private readonly Lazy<IComponentSerializerService> _serializers;
        private readonly EnginesRoot _enginesRoot;

        private DoubleDictionary<Id<IEntityType>, IEntityType, IEntityTypeProvider> _providers;
        private Dictionary<Id<IEntityType>, IEntityType> _types;
        private Dictionary<Type, object> _byDescriptor;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTypeService(
            IFiltered<IEntityInitializer> initializers,
            Lazy<IComponentSerializerService> serializers,
            EnginesRoot enginesRoot)
        {
            _initializers = initializers;
            _serializers = serializers;
            _enginesRoot = enginesRoot;

            // Load all IEntityType instances for which there is an initializer (or has been imported as a resource)
            IEnumerable<IEntityType> importedInitializers = Resource<IEntityType>.GetAll().Select(x => x.Value);
            _types = _initializers.SelectMany(init => init.ExplicitEntityTypes)
                .Concat(importedInitializers)
                .Distinct()
                .Where(x => x.Flags.HasFlag(EntityTypeFlags.Partial) == false)
                .ToDictionary(x => x.Id, x => x);

            _byDescriptor = new Dictionary<Type, object>();
        }

        public void Initialize(IEngineService engines)
        {
            IEntityFactory factory = _enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = _enginesRoot.GenerateEntityFunctions();
            EntityService entities = engines.Get<EntityService>();


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
                            factory,
                            functions,
                            engines,
                            _serializers.Value,
                            this.entitiesDB
                        );
                    });

            // IEntityType instances each get a single a Svelto entity automatically created here
            // The idea behind this entity is to contain "type" shared data that is consistent
            // Across all entities of this type.
            foreach (IEntityTypeProvider typeProvider in _providers.Values)
            {
                var data = EntityTypeHelper.GetData(typeProvider.Type);

                // Its a little messy, but we create an EntityId and add it to EntityService manually
                // This makes the type entity appear and behave as if it is like an instance entity.
                // Fully queryable within IEntityService as one would expect.
                // Likewise, EntityId is the primary key associated with filters, meaning type entites can be added to filters
                // Or hold filtered instances.
                EntityId typeEntityId = new EntityId(data.EGID, typeProvider.Type.Id.Value);

                // Configure global components
                // This parallels the actions done in EntityService for instance spawning
                EntityInitializer entityInitializer = factory.BuildEntity(data.EGID, typeProvider.Type.Descriptor.Type);
                entityInitializer.Init(typeEntityId);
                entityInitializer.Init(data.StaticComponent);
                entityInitializer.Init(typeProvider.Type.Id);
                entityInitializer.Init(typeProvider.Type.Descriptor.Id);

                typeProvider.InitializeType(entities, typeProvider.Type, in typeEntityId, ref entityInitializer);

                entities.AddId(typeEntityId);
            }
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

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TypeData> entities, ExclusiveGroupStruct groupID)
        {
            var (instances, ids, _) = entities;
            var (typeIds, _) = this.entitiesDB.QueryEntities<Id<IEntityType>>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                Id<IEntityType> typeId = typeIds[i];
                IEntityType type = this.GetById(typeId);

                EntityTypeHelper.SetGroupIndex(type, new GroupIndex(groupID, i));
            }
        }

        /// <summary>
        /// Automatically add instance entities to their respective
        /// static filters.
        /// </summary>
        /// <param name="rangeOfEntities"></param>
        /// <param name="entities"></param>
        /// <param name="groupID"></param>
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<InstanceData> entities, ExclusiveGroupStruct groupID)
        {
            var (instances, ids, _) = entities;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                InstanceData instanceComponent = instances[i];
                ref TypeData staticComponent = ref this.entitiesDB.QueryEntityByIndex<TypeData>(instanceComponent.StaticEntityId.Index, instanceComponent.StaticEntityId.GroupID);

                ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<EntityId>(staticComponent.InstanceEntitiesFilterId);
                filter.Add(ids[i], groupID, i);

                staticComponent.InstanceEntitiesCount++;
            }
        }

        public IEntityTypeProvider GetProviderByType(IEntityType type)
        {
            return _providers[type];
        }

        public IEntityTypeProvider GetProviderByTypeId(Id<IEntityType> id)
        {
            return _providers[id];
        }
    }
}
