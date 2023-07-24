using Svelto.ECS;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct Collision : IEntityComponent
    {
        public required CollisionGroup Categories { get; init; }
        public required CollisionGroup CollidesWith { get; init; } 

    }
}
