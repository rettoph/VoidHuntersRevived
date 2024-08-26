using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Interfaces
{
    public interface ICloneableComponent<TSelf> : IEntityComponent
        where TSelf : unmanaged, ICloneableComponent<TSelf>
    {
        TSelf Clone();
    }
}
