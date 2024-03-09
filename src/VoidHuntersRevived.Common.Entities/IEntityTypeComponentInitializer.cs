using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeComponentInitializer<T> : IDisposable
        where T : unmanaged, IEntityComponent
    {
        T GetInstance(EntityId id);
    }
}
