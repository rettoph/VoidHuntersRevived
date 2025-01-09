using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public readonly struct Collision : IEntityComponent
    {
        public required CollisionGroup Categories { get; init; }
        public required CollisionGroup CollidesWith { get; init; }
    }
}