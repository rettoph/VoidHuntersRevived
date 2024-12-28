using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Extensions.FixedPoint
{
    public static class FixTransform2DExtensions
    {
        public static Location ToLocation(this FixTransform2D transform)
        {
            return new Location(new FixVector2(transform.X, transform.Y), transform.Rotation);
        }
    }
}
