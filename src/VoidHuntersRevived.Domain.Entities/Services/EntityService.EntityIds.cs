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
    internal partial class EntityService
    {
        private readonly Dictionary<VhId, EntityId> _ids = new Dictionary<VhId, EntityId>();
        private readonly Queue<EntityId> _added = new Queue<EntityId>();

        public string name { get; } = nameof(EntityService);

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGetValue(vhid, out id);
        }

        private EntityId Add(EntityId id)
        {
            if(!_ids.TryAdd(id.VhId, id))
            {
                throw new Exception();
            }

            _added.Enqueue(id);

            return id;
        }

        private EntityId Remove(EntityId id)
        {
            _ids.Remove(id.VhId);

            return id;
        }
    }
}
