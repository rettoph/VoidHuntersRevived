using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IOnSpawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnSpawn(VhId sourceEventId, IEntityType type, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
