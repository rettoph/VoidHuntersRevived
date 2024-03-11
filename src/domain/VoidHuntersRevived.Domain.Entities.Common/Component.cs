using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public struct Component<T>
        where T : unmanaged, IEntityComponent
    {
        public readonly EntityId Id;
        public readonly T Value;

        public Component(EntityId id, T value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
