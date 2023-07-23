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
        private readonly Dictionary<EntityId, EntityState> _states = new Dictionary<EntityId, EntityState>();

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

        public EntityState GetState(EntityId id)
        {
            if(_states.TryGetValue(id, out EntityState state))
            {
                return state;
            }

            return EntityState.Unknown;
        }

        public void Step(in Step _param)
        {
            while (_removed.TryDequeue(out EntityId removed))
            {
                _ids.Remove(removed.VhId, removed.EGID);
                _states.Remove(removed);
            }
        }

        internal EntityId Add(VhId vhid, EGID egid)
        {
            EntityId id = new EntityId(egid, vhid);
            _ids.TryAdd(vhid, egid, id);
            _states.Add(id, EntityState.Spawned);

            return id;
        }

        internal EntityId Remove(VhId vhid)
        {
            ref EntityId id = ref _ids.TryGetRef(vhid, out bool isNullRef);
            if (isNullRef)
            {
                throw new NullReferenceException();
            }

            _removed.Enqueue(id);
            _states[id] = EntityState.Despawned;

            return id;
        }
    }
}
