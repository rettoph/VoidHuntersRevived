using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Physics.Extensions.FixedPoint
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
