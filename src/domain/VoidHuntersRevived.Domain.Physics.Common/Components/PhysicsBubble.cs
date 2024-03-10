using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public struct PhysicsBubble : IEntityComponent
    {
        public PhysicsBubble(Fix64 radius)
        {
            this.Radius = radius;
        }

        public required bool Enabled { get; init; }
        public required Fix64 Radius { get; init; }
    }
}
