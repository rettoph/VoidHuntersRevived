using Guppy.Core.Common;
using Guppy.Core.Common.Utilities;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    internal sealed class EntityTypeProvider : IEntityTypeProvider, IDisposable
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly EntityService _entities;
        private EntitiesDB _entitiesDB;
        private FasterList<ComponentEngineInvoker> _onDespawnEngineInvokers;
        private FasterList<ComponentEngineInvoker> _onSpawnEngineInvokers;

        private InstanceEntity _instanceEntityComponent;
        private BelongsTo<TypeEntity, InstanceEntity> _belongsToTypeInstanceEntityComponent;

        private FasterList<ComponentSerializer> _instanceEntityComponentSerializers;

        private DynamicEntityDescriptor<VoidHuntersEntityDescriptor> _instanceEntityDescriptor;
        private readonly ExclusiveGroupStruct _instanceEntityGroup;

        private DynamicEntityDescriptor<VoidHuntersEntityDescriptor> _typeEntityDescriptor;
        private readonly ExclusiveGroupStruct _typeEntityGroup;

        public IEntityType Type { get; }
        public IEntityType[] ImplementedTypes { get; }

        public ComponentBuilderDictionary InstanceEntityComponentBuilders { get; }
        public EntityInitializerDelegate? InstanceEntityInitializer { get; set; }
        public DisposeEntityInitializerDelegate? InstanceEntityDisposer { get; set; }

        public ComponentBuilderDictionary TypeEntityComponentBuilders { get; }
        public EntityInitializerDelegate? TypeEntityInitializer { get; set; }
        public DisposeEntityInitializerDelegate? TypeEntityDisposer { get; set; }

        public EntityTypeProvider(
            IEntityType type,
            IEntityTypeService entityTypeService,
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityFactory factory,
            IEntityFunctions functions,
            IFiltered<IEntityTypeProviderInitializer> entityTypeProviderInitializers,
            EntityService entityService
        )
        {
            this.Type = type;

            _typeRef = new UnmanagedReference<IEntityType>(this.Type);
            _uniqueNumberProvider = uniqueNumberProvider;
            _factory = factory;
            _functions = functions;
            _entities = entityService;
            _entitiesDB = null!;

            _onDespawnEngineInvokers = null!;
            _onSpawnEngineInvokers = null!;
            _instanceEntityComponentSerializers = null!;

            this.ImplementedTypes = EntityTypeProvider.GetImplementedTypes(type, entityTypeService).ToArray();

            // Import a dictionary of default component values based on all implemented types
            this.InstanceEntityComponentBuilders = new ComponentBuilderDictionary(this.ImplementedTypes.Select(x => x.InstanceEntityComponentBuilders));
            this.TypeEntityComponentBuilders = new ComponentBuilderDictionary(this.ImplementedTypes.Select(x => x.TypeEntityComponentBuilders));

            // Values are defined within initialization method
            this.InstanceEntityInitializer = null!;
            this.InstanceEntityDisposer = null!;
            this.TypeEntityDisposer = null!;
            this.TypeEntityDisposer = null!;

            // Invoke custom provider initialization
            foreach (IEntityTypeProviderInitializer entityTypeProviderInitializer in entityTypeProviderInitializers)
            {
                entityTypeProviderInitializer.InitializeEntityTypeProvider(this);
            }

            // Add custom front-to-back delegates
            foreach (IEntityTypeProviderInitializer entityTypeProviderInitializer in entityTypeProviderInitializers.OrderBy(x => x.Order))
            {
                this.InstanceEntityInitializer += entityTypeProviderInitializer.GetInstanceEntityInitializer(this);
                this.TypeEntityInitializer += entityTypeProviderInitializer.GetTypeEntityInitializer(this);
            }

            // Add custom back-to-front delegates
            foreach (IEntityTypeProviderInitializer entityTypeProviderInitializer in entityTypeProviderInitializers.OrderByDescending(x => x.Order))
            {
                this.InstanceEntityDisposer += entityTypeProviderInitializer.GetInstanceEntityDisposer(this);
                this.TypeEntityDisposer += entityTypeProviderInitializer.GetTypeEntityDisposer(this);
            }

            this.InstanceEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.InstanceEntityDisposer ??= EntityTypeProvider.DefaultDisposer;
            this.TypeEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.TypeEntityDisposer ??= EntityTypeProvider.DefaultDisposer;

            // BEGIN POST INITIALIZATION
            // Once the entity type has been configured for the scope we can 
            // Finalize initialization - create svelto descriptors, build automated engines
            // ect...

            // This is very lowkey, but this is responsible for spawning the primary TypeEntity instance for the
            // Current provider's type.

            //  Build type svelto descriptors
            if (EntityTypeProvider.ValidateRequiredComponents(this.TypeEntityComponentBuilders, this.ImplementedTypes, x => x.RequiredTypeEntityComponents, out Type[] missingTypes) == false)
            {
                throw new EntityProviderTypeException(type.Key, $"Exception building {type.Key} provider - missing the following required {nameof(this.TypeEntityComponentBuilders)}: {string.Join(',', missingTypes.Select(x => x.GetFormattedName()))}");
            }

            _typeEntityDescriptor = new DynamicEntityDescriptor<VoidHuntersEntityDescriptor>(this.TypeEntityComponentBuilders);
            _typeEntityGroup = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{this.Type.Key}_{nameof(_typeEntityGroup)}");

            this.SpawnTypeEntity(out _belongsToTypeInstanceEntityComponent);
            _instanceEntityComponent = new InstanceEntity(_typeRef);

            // Update instance components with local strategy data
            this.InstanceEntityComponentBuilders.Set(_instanceEntityComponent);
            this.InstanceEntityComponentBuilders.Set(_belongsToTypeInstanceEntityComponent);

            // Build instance svelto descriptors
            if (EntityTypeProvider.ValidateRequiredComponents(this.InstanceEntityComponentBuilders, this.ImplementedTypes, x => x.RequiredInstanceEntityComponents, out missingTypes) == false)
            {
                throw new EntityProviderTypeException(type.Key, $"Exception building {type.Key} provider - missing the following required {nameof(this.InstanceEntityComponentBuilders)}: {string.Join(',', missingTypes.Select(x => x.GetFormattedName()))}");
            }
            _instanceEntityDescriptor = new DynamicEntityDescriptor<VoidHuntersEntityDescriptor>(this.InstanceEntityComponentBuilders);
            _instanceEntityGroup = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{this.Type.Key}_{nameof(_instanceEntityGroup)}");
        }

        public void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService,
            IFiltered<IEntityTypeProviderInitializer> entityTypeProviderInitializers)
        {
            _entitiesDB = entitiesDB;

            // Load serializers
            IEnumerable<Type> instanceEntityComponentTypes = _instanceEntityDescriptor.componentsToBuild.Select(x => x.GetEntityComponentType());
            ComponentSerializer[] instanceEntityComponentSerializers = componentSerializerService.GetComponentSerializers(instanceEntityComponentTypes).ToArray();
            _instanceEntityComponentSerializers = new FasterList<ComponentSerializer>(instanceEntityComponentSerializers);

            // Generate despawn engine invokers
            // Responsible for calling IOnSpawnEngine & IOnDespawnEngine engines
            _onDespawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _onSpawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            foreach (Type componentType in this.Type.InstanceEntityComponentBuilders.Keys)
            {
                if (ComponentEngineInvoker.Create(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), componentType, engineService.All(), out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }

                if (ComponentEngineInvoker.Create(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), componentType, engineService.All(), out invoker))
                {
                    _onSpawnEngineInvokers.Add(invoker);
                }
            }
        }

        public void Dispose()
        {
            _typeRef.Dispose(false);
        }

        #region Instance Entity Methods
        public EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in VhId vhid, out EntityId id)
        {
            // Create a new EGID for the entity
            EGID egid = new EGID(_uniqueNumberProvider.GetUInt32(), _instanceEntityGroup);
            id = new EntityId(egid, vhid);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = _factory.BuildEntity(egid, _instanceEntityDescriptor);
            initializer.Init(id);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));

            // Run custom instance initializer
            this.InstanceEntityInitializer!(_entities, this, id, ref initializer);

            return initializer;
        }

        public void SoftSpawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            // Call all OnSpawn engines
            for (int i = 0; i < _onSpawnEngineInvokers.count; i++)
            {
                _onSpawnEngineInvokers[i].Invoke(sourceEventId, this.Type, _entitiesDB, id, groupIndex);
            }
        }

        public void SoftDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            // Call all OnDespawn engines
            for (int i = 0; i < _onDespawnEngineInvokers.count; i++)
            {
                _onDespawnEngineInvokers[i].Invoke(sourceEventId, this.Type, _entitiesDB, id, groupIndex);
            }
        }

        public void HardDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _functions.RemoveEntity<VoidHuntersEntityDescriptor>(id.EGID);
        }

        public void SerializeInstanceEntity(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (ComponentSerializer serializer in _instanceEntityComponentSerializers)
            {
                serializer.Serialize(writer, in groupIndex, _entitiesDB, in options);
            }
        }

        public void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _instanceEntityComponentSerializers)
            {
                serializer.Deserialize(in sourceId, in options, reader, ref initializer, in id);
            }
        }
        #endregion

        #region Type Entity Methods
        private EntityId SpawnTypeEntity(out BelongsTo<TypeEntity, InstanceEntity> belongsToTypeInstanceEntityComponent)
        {
            // IEntityType instances each get a single a Svelto entity automatically created here
            // The idea behind this entity is to contain "type" shared data that is consistent
            // Across all entities of this type.

            // Its a little messy, but we create an EntityId and add it to EntityService manually
            // This makes the type entity appear and behave as if it is like an instance entity.
            // Fully queryable within IEntityService as one would expect.
            // Likewise, EntityId is the primary key associated with filters, meaning type entites can be added to filters
            // Or hold filtered instances.
            // Create a new EGID for the entity
            EGID egid = new EGID(_uniqueNumberProvider.GetUInt32(), _typeEntityGroup);
            EntityId id = new EntityId(egid, this.Type.Key.Id);

            // Configure global components
            // This parallels the actions done in EntityService for instance spawning
            EntityInitializer initializer = _factory.BuildEntity(egid, _typeEntityDescriptor);
            initializer.Init(id);

            TypeEntity typeEntityComponent = new TypeEntity(_typeRef);
            initializer.Init(typeEntityComponent);

            this.TypeEntityInitializer!(_entities, this, id, ref initializer);

            // These instance components are automatically applied to all created instance entities
            belongsToTypeInstanceEntityComponent = new BelongsTo<TypeEntity, InstanceEntity>(id.VhId);

            _entities.Query.AddId(id);

            return id;
        }
        #endregion

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            return _instanceEntityDescriptor.componentsToBuild.Select(x => x.GetEntityComponentType())
                .Concat(_typeEntityDescriptor.componentsToBuild.Select(x => x.GetEntityComponentType()))
                .Distinct();
        }

        public bool Implements(IKey<IEntityType> key)
        {
            foreach (IEntityType implementedType in this.ImplementedTypes)
            {
                if (key == implementedType.Key)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DefaultInitializer(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityTypeProvider provider)
        {
            // throw new NotImplementedException();
        }

        private static IEnumerable<IEntityType> GetImplementedTypes(IEntityType entityType, IEntityTypeService entityTypeService, HashSet<IEntityType> result = null)
        {
            result ??= new HashSet<IEntityType>();

            if (result.Contains(entityType) == true)
            {
                return result;
            }

            foreach (IKey<IEntityType> includedKey in entityType.Include)
            {
                IEntityType includedType = entityTypeService.GetByKey(includedKey);

                EntityTypeProvider.GetImplementedTypes(includedType, entityTypeService, result);
            }

            if (result.Add(entityType) == false)
            {
                return result;
            }

            return result;
        }

        private static bool ValidateRequiredComponents<T>(
            ComponentBuilderDictionary componentBuilders,
            IEnumerable<IEntityType> importedTypes,
            Func<IEntityType, T> memberExpression,
            out Type[] missingTypes)
                where T : HashSet<Type>
        {
            Type[] requiredTypes = importedTypes.SelectMany(x => memberExpression(x)).Distinct().ToArray();
            missingTypes = requiredTypes.Except(componentBuilders.Keys).ToArray();

            return missingTypes.Length == 0;
        }
    }
}
