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
        private readonly EntityTypeService _entityTypes;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly SimpleEntitiesSubmissionScheduler _submission;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _ids;
        private readonly Dictionary<VhId, EntityType> _types;
        private readonly Queue<IdMap> _removed;
        private uint _id;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(
            EntityTypeService entityTypes,
            IEntityFactory factory,
            IEntityFunctions functions,
            SimpleEntitiesSubmissionScheduler sumbission)
        {
            _entityTypes = entityTypes;
            _factory = factory;
            _functions = functions;
            _submission = sumbission;
            _ids = new DoubleDictionary<VhId, EGID, IdMap>();
            _removed = new Queue<IdMap>();
            _types = new Dictionary<VhId, EntityType>();
        }

        public void Ready()
        {
        }

        public IdMap Create(EntityType type, VhId vhid)
        {
            EntityInitializer initializer = type.CreateEntity(_factory);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _ids.TryAdd(vhid, initializer.EGID, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        public IdMap Create(EntityType type, VhId vhid, EntityInitializerDelegate initializerDelegate)
        {
            EntityInitializer initializer = type.CreateEntity(_factory);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(ref initializer);

            initializerDelegate(ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _ids.TryAdd(vhid, initializer.EGID, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        public void Destroy(VhId vhid)
        {
            if (!this.TryGetIdMap(ref vhid, out IdMap id))
            {
                return;
            }

            _types[vhid].DestroyEntity(_functions, in id.EGID);
            _removed.Enqueue(id);
        }

        public bool TryGetIdMap(ref VhId vhid, out IdMap id)
        {
            return _ids.TryGet(vhid, out id);
        }

        public IdMap GetIdMap(VhId vhid)
        {
            return _ids[vhid];
        }

        public IdMap GetIdMap(EGID egid)
        {
            return _ids[egid];
        }

        public IdMap GetIdMap(uint id, ExclusiveGroupStruct group)
        {
            return this.GetIdMap(new EGID(id, group));
        }

        public void Clean()
        {
            _submission.SubmitEntities();

            while (_removed.TryDequeue(out IdMap removed))
            {
                _ids.Remove(removed.VhId, removed.EGID);
                _types.Remove(removed.VhId);
            }
        }

        public EntityType GetEntityType(VhId entityVhId)
        {
            return _types[entityVhId];
        }
    }
}
