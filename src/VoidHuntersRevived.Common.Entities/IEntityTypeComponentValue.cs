using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeComponentValue<T> : IDisposable
        where T : unmanaged, IEntityComponent
    {
        T GetInstance(EntityId id);
    }
}
