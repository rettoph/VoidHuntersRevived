using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeService : BasicEngine, IEntityTypeService, IQueryingEntitiesEngine, IReactOnAddEx<InstanceData>, IReactOnAddEx<TypeData>
    {
        private Dictionary<Id<IEntityType>, IEntityType> _types;
        private Dictionary<Type, object> _byDescriptor;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTypeService(IEntityTypeInitializerService typeInitializersService, EnginesRoot enginesRoot, EntityService entities)
        {
            // Fetch all non partial types
            List<IEntityTypeInitializer> typeInitializers = typeInitializersService.GetAll(EntityTypeFlags.Partial).ToList();

            // Store them internally
            _types = typeInitializers.Select(x => x.Type).ToDictionary(x => x.Id, x => x);
            _byDescriptor = new Dictionary<Type, object>();


            // IEntityType instances each get a single a Svelto entity automatically created here
            // The idea behind this entity is to contain "type" shared data that is consistent
            // Across all entities of this type.
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            foreach (IEntityTypeInitializer typeInitializer in typeInitializersService.GetAll(EntityTypeFlags.Partial))
            {
                var data = EntityTypeHelper.GetData(typeInitializer.Type);

                // Its a little messy, but we create an EntityId and add it to EntityService manually
                // This makes the type entity appear and behave as if it is like an instance entity.
                // Fully queryable within IEntityService as one would expect.
                // Likewise, EntityId is the primary key associated with filters, meaning type entites can be added to filters
                // Or hold filtered instances.
                EntityId typeEntityId = new EntityId(data.EGID, typeInitializer.Type.Id.Value);

                // Configure global components
                // This parallels the actions done in EntityService for instance spawning
                EntityInitializer entityInitializer = factory.BuildEntity(data.EGID, typeInitializer.Type.Descriptor.StaticDescriptor);
                entityInitializer.Init(typeEntityId);
                entityInitializer.Init(data.StaticComponent);
                entityInitializer.Init(typeInitializer.Type.Id);
                entityInitializer.Init(typeInitializer.Type.Descriptor.Id);

                typeInitializer.InitializeType(entities, typeInitializer.Type, in typeEntityId, ref entityInitializer);

                entities.AddId(typeEntityId);
            }
        }

        public IEntityType GetById(Id<IEntityType> id)
        {
            return _types[id];
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
    }
}
