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
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    internal sealed class EntityTypeProvider : IEntityTypeProvider
    {
        internal static AsyncLocal<uint> EntityId = new AsyncLocal<uint>();


        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly IEntityService _entities;
        private readonly EntitiesDB _entitiesDB;
        private readonly FasterList<ComponentEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentEngineInvoker> _onSpawnEngineInvokers;

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

            _factory = factory;
            _functions = functions;
            _entities = engines.Get<IEntityService>();
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
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region Instance Methods
        private static void DefaultInitializer(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityType type)
        {
            // throw new NotImplementedException();
        }

        public EntityInitializer HardSpawnInstance(in VhId sourceEventId, in VhId vhid, out EntityId id)
        {
            // Create a new EGID for the entity
            EGID egid = new EGID(EntityId.Value++, this.Type.Descriptor.InstanceGroup);
            id = new EntityId(egid, vhid);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = _factory.BuildEntity(egid, this.Type.Descriptor.Instance);
            initializer.Init(id);
            initializer.Init(this.Type.Id);
            initializer.Init(this.Type.Descriptor.Id);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));

            // Run custom instance initializer
            this.InstanceEntityInitializer!(_entities, Type, in id, ref initializer);

            return initializer;
        }

        public void SoftSpawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            // Call all OnSpawn engines
            for (int i = 0; i < _onSpawnEngineInvokers.count; i++)
            {
                _onSpawnEngineInvokers[i].Invoke(sourceEventId, _entitiesDB, id, groupIndex);
            }
        }

        public void SoftDespawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            // Call all OnDespawn engines
            for (int i = 0; i < _onDespawnEngineInvokers.count; i++)
            {
                _onDespawnEngineInvokers[i].Invoke(sourceEventId, _entitiesDB, id, groupIndex);
            }
        }

        public void HardDespawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _functions.RemoveEntity<InstanceEntityDescriptor>(id.EGID);
        }

        public void SerializeInstance(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, in groupIndex, _entitiesDB, in options);
            }
        }

        public void DeserializeInstance(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(in sourceId, in options, reader, ref initializer, in id);
            }
        }
        #endregion

        #region Type Methods
        public void InitializeType(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            TypeEntityInitializer!(entities, Type, in id, ref initializer);
        }
        #endregion
    }
}
