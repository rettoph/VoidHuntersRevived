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
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Services;
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
        private readonly FasterList<ComponentEngineInvoker> _onDespawnEngineInvokers;
        private readonly FasterList<ComponentEngineInvoker> _onSpawnEngineInvokers;
        private readonly EntityInitializerDelegate _initializer;
        private EntitiesDB _entitiesDB;
        private FasterList<ComponentSerializer> _serializers;

        private DynamicEntityDescriptor<VoidHuntersEntityDescriptor> _descriptor;
        private readonly ExclusiveGroupStruct _group;

        public IEntityType Type { get; }

        public EntityTypeProvider(
            IEntityType type,
            IEntityTypeService entityTypeService,
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityFactory factory,
            IEntityFunctions functions,
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

            _onDespawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _onSpawnEngineInvokers = new FasterList<ComponentEngineInvoker>();
            _serializers = null!;

            _initializer = this.Type.Initializer ?? EntityTypeProvider.DefaultInitializer;

            _descriptor = new DynamicEntityDescriptor<VoidHuntersEntityDescriptor>(this.Type.Components);
            _group = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{this.Type.Key}_{nameof(_group)}");
        }

        public void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService)
        {
            _entitiesDB = entitiesDB;

            // Load serializers
            _serializers = componentSerializerService.GetComponentSerializersByDescriptor(_descriptor);

            // Generate despawn engine invokers
            // Responsible for calling IOnSpawnEngine & IOnDespawnEngine engines
            foreach (Type componentType in this.Type.Components.Keys)
            {
                if (ComponentEngineInvoker.Create(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), componentType, engineService, out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }

                if (ComponentEngineInvoker.Create(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), componentType, engineService, out invoker))
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
            EGID egid = new EGID(_uniqueNumberProvider.GetUInt32(), _group);
            id = new EntityId(egid, vhid);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = _factory.BuildEntity(egid, _descriptor);
            initializer.Init(id);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));

            // Run custom instance initializer
            _initializer!(_entities, this.Type, id, ref initializer);

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

        public void SerializeInstanceEntity(EntityWriter writer, in EntityId id, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, in id, in groupIndex, _entitiesDB, in options);
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

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            return _descriptor.componentsToBuild.Select(x => x.GetEntityComponentType())
                .Distinct();
        }

        private static void DefaultInitializer(IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityType entityType)
        {
            // throw new NotImplementedException();
        }

        private static IEnumerable<IEntityType> GetImplementedTypes(IEntityType entityType, IEntityTypeService entityTypeService, HashSet<IEntityType>? result = null)
        {
            result ??= [];

            if (result.Contains(entityType) == true)
            {
                return result;
            }

            // foreach (Key<IEntityType> includedKey in entityType.Include)
            // {
            //     IEntityType includedType = entityTypeService.GetByKey(includedKey);
            // 
            //     EntityTypeProvider.GetImplementedTypes(includedType, entityTypeService, result);
            // }

            if (result.Add(entityType) == false)
            {
                return result;
            }

            return result;
        }
    }
}
