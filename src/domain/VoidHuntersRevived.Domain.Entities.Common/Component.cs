using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public struct Component<T>(EntityId id, T value)
        where T : unmanaged, IEntityComponent
    {
        public readonly EntityId Id = id;
        public readonly T Value = value;
    }
}
