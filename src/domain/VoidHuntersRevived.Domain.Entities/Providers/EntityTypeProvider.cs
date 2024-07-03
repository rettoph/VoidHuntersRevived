using Guppy.Core.Common.Utilities;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
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
        internal static AsyncLocal<uint> EntityId = new AsyncLocal<uint>();

        private readonly UnmanagedReference<IEntityType> _typeRef;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly EntityService _entities;
        private readonly EntitiesDB _entitiesDB;
        private readonly FasterList<ComponentEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentEngineInvoker> _onSpawnEngineInvokers;

        private readonly InstanceEntity _instanceEntityComponent;
        private readonly BelongsTo<TypeEntity, InstanceEntity> _belongsToTypeInstanceEntityComponent;

        private readonly FasterList<ComponentSerializer> _serializers;

        // Despite being nullable these delegates will have a default value defined
        // within the constructor. No need to check
        public EntityInitializerDelegate? InstanceEntityInitializer;
        public EntityInitializerDelegate? TypeEntityInitializer;

        public DisposeEntityInitializerDelegate? InstanceEntityDisposer;
        public DisposeEntityInitializerDelegate? TypeEntityDisposer;

        public IEntityType Type { get; }

        public EntityTypeProvider(
            IEntityType type,
            IEnumerable<IEntityInitializer> initializers,
            IEntityFactory factory,
            IEntityFunctions functions,
            IEngineService engines,
            IComponentSerializerService serializers,
            EntitiesDB entitiesDB
        )
        {
            this.Type = type;

            _typeRef = new UnmanagedReference<IEntityType>(this.Type);
            _factory = factory;
            _functions = functions;
            _entities = engines.Get<EntityService>();
            _entitiesDB = entitiesDB;
            _serializers = serializers.GetInstanceComponentSerializers(this.Type);

            // Create a list of all OnDespawn & OnSpawn engines for components contained within the current
            // TypeProvider Type
            _onDespawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _onSpawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            foreach (Type componentType in this.Type.Descriptor.Instance.componentsToBuild.Select(x => x.GetEntityComponentType()))
            {
                if (ComponentEngineInvoker.Create(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), componentType, engines.All(), out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }

                if (ComponentEngineInvoker.Create(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), componentType, engines.All(), out invoker))
                {
                    _onSpawnEngineInvokers.Add(invoker);
                }
            }

            // Append hard coded component initialization delegates
            this.InstanceEntityInitializer += EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.InstanceComponents.Values);
            this.TypeEntityInitializer += EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.Components.Values);

            // Add relevent front-to-back delegates
            foreach (IEntityInitializer initializer in initializers.OrderBy(x => x.Order))
            {
                this.InstanceEntityInitializer += initializer.InstanceInitializer(Type);
                this.TypeEntityInitializer += initializer.TypeInitializer(Type);
            }

            // Add relevent back-to-front delegates
            foreach (IEntityInitializer initializer in initializers.OrderByDescending(x => x.Order))
            {
                this.InstanceEntityDisposer += initializer.InstanceDisposer(Type);
                this.TypeEntityDisposer += initializer.TypeDisposer(Type);
            }

            // Add some default values if no delegates were defined.
            this.InstanceEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.InstanceEntityDisposer ??= EntityTypeProvider.DefaultDisposer;
            this.TypeEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.TypeEntityDisposer ??= EntityTypeProvider.DefaultDisposer;

            // This is very lowkey, but this is responsible for spawning the primary TypeEntity instance for the
            // Current provider's type.
            this.SpawnTypeEntity(out _belongsToTypeInstanceEntityComponent);
            _instanceEntityComponent = new InstanceEntity(_typeRef);

        }

        public void Dispose()
        {
            _typeRef.Dispose(false);
        }

        #region Instance Entity Methods
        public EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in VhId vhid, out EntityId id)
        {
            // Create a new EGID for the entity
            EGID egid = new EGID(EntityId.Value++, this.Type.Descriptor.InstanceGroup);
            id = new EntityId(egid, vhid);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = _factory.BuildEntity(egid, this.Type.Descriptor.Instance);
            initializer.Init(id);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));
            initializer.Init(_instanceEntityComponent);
            initializer.Init(_belongsToTypeInstanceEntityComponent);

            // Run custom instance initializer
            this.InstanceEntityInitializer!(_entities, Type, id, ref initializer);

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
            _functions.RemoveEntity<InstanceEntityDescriptor>(id.EGID);
        }

        public void SerializeInstanceEntity(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, in groupIndex, _entitiesDB, in options);
            }
        }

        public void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
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
            EGID egid = new EGID(EntityId.Value++, this.Type.Descriptor.TypeGroup);
            EntityId id = new EntityId(egid, this.Type.Id.Value);

            // Configure global components
            // This parallels the actions done in EntityService for instance spawning
            EntityInitializer initializer = _factory.BuildEntity(egid, this.Type.Descriptor.Type);
            initializer.Init(id);

            TypeEntity typeEntityComponent = new TypeEntity(_typeRef);
            initializer.Init(typeEntityComponent);

            this.TypeEntityInitializer!(_entities, Type, id, ref initializer);

            // These instance components are automatically applied to all created instance entities
            belongsToTypeInstanceEntityComponent = new BelongsTo<TypeEntity, InstanceEntity>(id.VhId);

            _entities.AddId(id);

            return id;
        }
        #endregion

        private static void DefaultInitializer(IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityType type)
        {
            // throw new NotImplementedException();
        }
    }
}
