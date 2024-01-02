using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly Dictionary<VhId, EntityId> _ids = new Dictionary<VhId, EntityId>();
        private readonly Queue<EventDto> _modifications = new Queue<EventDto>();

        public string name { get; } = nameof(EntityService);

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGetValue(vhid, out id);
        }

        private ref EntityId GetOrAddId(VhId vhid, out bool exists)
        {
            return ref CollectionsMarshal.GetValueRefOrAddDefault(_ids, vhid, out exists);
        }

        private bool AddId(EntityId id)
        {
            if (_ids.TryAdd(id.VhId, id))
            {
                return true;
            }

            throw new Exception();
        }

        private bool RemoveId(EntityId id)
        {
            if (_ids.Remove(id.VhId))
            {
                return true;
            }

            throw new Exception();
        }
    }
}
