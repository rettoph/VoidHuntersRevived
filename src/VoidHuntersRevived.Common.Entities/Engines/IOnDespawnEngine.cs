using Svelto.ECS;
using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnDespawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnDespawn(VhId sourceEventId, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
