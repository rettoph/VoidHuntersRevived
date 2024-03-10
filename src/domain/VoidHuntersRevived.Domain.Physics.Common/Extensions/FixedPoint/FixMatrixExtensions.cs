using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Extensions.FixedPoint
{
    public static class FixMatrixExtensions
    {
        public static Location ToLocation(this FixMatrix matrix)
        {
            return new Location()
            {
                Position = FixVector2.Transform(FixVector2.Zero, matrix),
                Rotation = matrix.Radians()
            };
        }
    }
}
