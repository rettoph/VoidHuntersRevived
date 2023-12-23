using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnDespawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnDespawn(EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
