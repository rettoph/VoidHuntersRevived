using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
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
