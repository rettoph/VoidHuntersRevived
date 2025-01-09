using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public readonly struct PhysicsBubble(Fix64 radius) : IEntityComponent
    {
        public required bool Enabled { get; init; }
        public required Fix64 Radius { get; init; } = radius;
    }
}