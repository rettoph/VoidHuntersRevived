using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IOnDespawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnDespawn(VhId sourceEventId, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
