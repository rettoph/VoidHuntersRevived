using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct Position2D : IEntityComponent
    {
        public FixVector2 Value;

        public Position2D(FixVector2 value)
        {
            this.Value = value;
        }
    }
}
