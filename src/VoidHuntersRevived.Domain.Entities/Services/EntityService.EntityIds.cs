using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common;
using Guppy.Common.Collections;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService : IStepEngine<Step>
    {
        private readonly DoubleDictionary<VhId, EGID, EntityId> _ids = new DoubleDictionary<VhId, EGID, EntityId>();
        private readonly Queue<EntityId> _removed = new Queue<EntityId>();

        public string name { get; } = nameof(EntityService);

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public EntityId GetId(EGID egid)
        {
            return _ids[egid];
        }

        public EntityId GetId(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.GetId(new EGID(entityId, groupId));
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGet(vhid, out id);
        }

        public bool TryGetId(EGID egid, out EntityId id)
        {
            return _ids.TryGet(egid, out id);
        }

        public bool TryGetId(uint entityId, ExclusiveGroupStruct groupId, out EntityId id)
        {
            return _ids.TryGet(new EGID(entityId, groupId), out id);
        }

        public void Step(in Step _param)
        {
            while (_removed.TryDequeue(out EntityId removed))
            {
                _ids.Remove(removed.VhId, removed.EGID);
            }
        }

        internal EntityId Add(VhId vhid, EGID egid)
        {
            EntityId idMap = new EntityId(egid, vhid);
            _ids.TryAdd(vhid, egid, idMap);

            return idMap;
        }

        internal EntityId Remove(VhId vhid)
        {
            ref EntityId id = ref _ids.TryGetRef(vhid, out bool isNullRef);
            if (isNullRef)
            {
                throw new NullReferenceException();
            }

            id.Destroyed = true;
            _removed.Enqueue(id);

            return id;
        }
    }
}
