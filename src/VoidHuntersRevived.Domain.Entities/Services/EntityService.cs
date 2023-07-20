using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Serilog;
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

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<StepSequence>(StepSequence.OnEntitySubmit)]
    internal sealed partial class EntityService : IEntityService, IEngine, IStepEngine<Step>
    {
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _ids;
        private readonly Dictionary<VhId, IEntityType> _types;
        private readonly Queue<IdMap> _removed;

        public string name { get; } = nameof(EntityService);

        public EntityService(SimpleEntitiesSubmissionScheduler scheduler)
        {
            _ids = new DoubleDictionary<VhId, EGID, IdMap>();
            _types = new Dictionary<VhId, IEntityType>();
            _removed = new Queue<IdMap>();
            _scheduler = scheduler;
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

        public IEntityType GetEntityType(VhId entityVhId)
        {
            return _types[entityVhId];
        }

        public void Clean()
        {
            while(_removed.TryDequeue(out IdMap removed))
            {
                _ids.Remove(removed.VhId, removed.EGID);
                _types.Remove(removed.VhId);
            }
        }

        public void Step(in Step _param)
        {
            _scheduler.SubmitEntities();
        }

        internal IdMap Add(VhId vhid, EGID egid, IEntityType type)
        {
            IdMap idMap = new IdMap(egid, vhid);
            _ids.TryAdd(vhid, egid, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        internal IdMap Remove(VhId vhid, out IEntityType type)
        {
            ref IdMap id = ref _ids.TryGetRef(vhid, out bool isNullRef);
            if (isNullRef)
            {
                throw new NullReferenceException();
            }

            id.Destroyed = true;
            _removed.Enqueue(id);
            type = _types[vhid];

            return id;
        }
    }
}
