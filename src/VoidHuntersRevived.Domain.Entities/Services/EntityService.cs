using Guppy.Common.Collections;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using Svelto.ECS.Serialization;
using System.Text;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityService : IEntityService, IQueryingEntitiesEngine
    {
        private readonly IWorld _world;
        private readonly EntityTypeService _entityTypes;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly SimpleEntitiesSubmissionScheduler _submission;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _ids;
        private readonly Dictionary<VhId, EntityType> _types;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(
            IWorld world,
            EntityTypeService entityTypes,
            IEntityFactory factory,
            IEntityFunctions functions,
            SimpleEntitiesSubmissionScheduler sumbission)
        {
            _world = world;
            _entityTypes = entityTypes;
            _factory = factory;
            _functions = functions;
            _submission = sumbission;
            _ids = new DoubleDictionary<VhId, EGID, IdMap>();
            _types = new Dictionary<VhId, EntityType>();
        }

        public void Ready()
        {
        }

        public IdMap Create(EntityType type, VhId vhid)
        {
            EntityInitializer initializer = type.CreateEntity(_factory);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(_world, ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _ids.TryAdd(vhid, initializer.EGID, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        public IdMap Create(EntityType type, VhId vhid, EntityInitializerDelegate initializerDelegate)
        {
            EntityInitializer initializer = type.CreateEntity(_factory);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(_world, ref initializer);

            initializerDelegate(_world, ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _ids.TryAdd(vhid, initializer.EGID, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        public void Destroy(VhId vhid)
        {
            if (!this.TryGetIdMap(vhid, out IdMap id))
            {
                return;
            }

            _types[vhid].DestroyEntity(_functions, in id.EGID);

            _ids.Remove(id.VhId, id.EGID);
            _types.Remove(id.VhId);
        }

        public IdMap GetIdMap(VhId vhid)
        {
            return _ids[vhid];
        }

        public IdMap GetIdMap(EGID egid)
        {
            return _ids[egid];
        }

        public IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.GetIdMap(new EGID(entityId, groupId));
        }

        public bool TryGetIdMap(VhId vhid, out IdMap id)
        {
            return _ids.TryGet(vhid, out id);
        }

        public bool TryGetIdMap(EGID egid, out IdMap id)
        {
            return _ids.TryGet(egid, out id);
        }

        public bool TryGetIdMap(uint entityId, ExclusiveGroupStruct groupId, out IdMap id)
        {
            return _ids.TryGet(new EGID(entityId, groupId), out id);
        }

        public void Clean()
        {
            _submission.SubmitEntities();
        }

        public EntityType GetEntityType(VhId entityVhId)
        {
            return _types[entityVhId];
        }
    }
}
