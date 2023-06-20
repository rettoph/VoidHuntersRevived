using Guppy.Common.Collections;
using Svelto.ECS;
using Svelto.ECS.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityService : IEntityService, IQueryingEntitiesEngine
    {
        private readonly EntityTypeService _entityTypes;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly IEntitySerialization _serialization;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _idMap;
        private uint _id;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(
            EntityTypeService entityTypes,
            IEntityFactory factory,
            IEntityFunctions functions,
            IEntitySerialization serialization)
        {
            _entityTypes = entityTypes;
            _factory = factory;
            _functions = functions;
            _serialization = serialization;
            _idMap = new DoubleDictionary<VhId, EGID, IdMap>();
        }

        public void Ready()
        {
        }

        public IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId vhid)
            where TDescriptor : IEntityDescriptor, new()
        {
            EntityInitializer initializer = _factory.BuildEntity<TDescriptor>(_id++, EntityType<TDescriptor>.Group);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _idMap.TryAdd(vhid, initializer.EGID, idMap);

            return idMap;
        }

        public IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId vhid, EntityInitializerDelegate initializerDelegate)
            where TDescriptor : IEntityDescriptor, new()
        {
            EntityInitializer initializer = _factory.BuildEntity<TDescriptor>(_id++, EntityType<TDescriptor>.Group);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(ref initializer);

            initializerDelegate(ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _idMap.TryAdd(vhid, initializer.EGID, idMap);

            return idMap;
        }

        public void Destroy<TDescriptor>(VhId vhid)
            where TDescriptor : IEntityDescriptor, new()
        {
            if(!this.TryGetIdMap(ref vhid, out IdMap id))
            {
                return;
            }

            _functions.RemoveEntity<TDescriptor>(id.EGID);
            _idMap.Remove(id.VhId, id.EGID);
        }

        public bool TryGetIdMap(ref VhId vhid, out IdMap id)
        {
            return _idMap.TryGet(vhid, out id);
        }

        public IdMap GetIdMap(VhId vhid)
        {
            return _idMap[vhid];
        }

        public IdMap GetIdMap(EGID egid)
        {
            return _idMap[egid];
        }

        public IdMap GetIdMap(uint id, ExclusiveGroupStruct group)
        {
            return this.GetIdMap(new EGID(id, group));
        }
    }
}
