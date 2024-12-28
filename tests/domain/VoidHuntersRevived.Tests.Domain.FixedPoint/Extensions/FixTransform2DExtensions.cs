using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions
{
    public static class FixTransform2DExtensions
    {
        private const float Epsilon = 0.0001f;

        public static void AssetEqualTo(this FixTransform2D source, Matrix target)
        {
            var (targetX, targetY, targetCos, targetSin) = target;

            float sourceX = (float)source.X;
            float sourceY = (float)source.Y;
            float sourceCos = (float)source.Cos;
            float sourceSin = (float)source.Sin;

            sourceX.AssertWithinEpsilon(targetX, Epsilon);
            sourceY.AssertWithinEpsilon(targetY, Epsilon);
            sourceCos.AssertWithinEpsilon(targetCos, Epsilon);
            sourceSin.AssertWithinEpsilon(targetSin, Epsilon);
        }
    }
}
