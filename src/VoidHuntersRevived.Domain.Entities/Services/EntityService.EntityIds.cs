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
        private enum EntityModification
        {
            SoftSpawn,
            SoftDespawn,
            RevertSoftDespawn,
            HardDespawn
        }

        private readonly Dictionary<VhId, EntityId> _ids = new Dictionary<VhId, EntityId>();
        private readonly Queue<(EntityModification, EntityId)> _modifications = new Queue<(EntityModification, EntityId)>();

        public string name { get; } = nameof(EntityService);

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGetValue(vhid, out id);
        }

        private bool AddId(EntityId id)
        {
            if(_ids.TryAdd(id.VhId, id))
            {
                return true;
            }

            throw new Exception();
        }

        private void EnqueuEntityModification(EntityModification type, EntityId id)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityModification = {EntityModification}, EntityId = {EntityId}", nameof(EntityService), nameof(EnqueuEntityModification), type, id.VhId);
            _modifications.Enqueue((type, id));
        }

        private bool RemoveId(EntityId id)
        {
            if(_ids.Remove(id.VhId))
            {
                return true;
            }

            throw new Exception();
        }
    }
}
