using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IOnSpawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnSpawn(VhId sourceEventId, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
