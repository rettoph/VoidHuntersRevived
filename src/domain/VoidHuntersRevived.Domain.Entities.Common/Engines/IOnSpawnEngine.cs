using Svelto.ECS;
using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnSpawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnSpawn(VhId sourceEventId, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
