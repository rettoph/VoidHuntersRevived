using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions
{
    public static class FixVector2Extensions
    {
        private const float _epsilon = 0.0001f;

        public static void AssetEqualTo(this FixVector2 source, Vector2 target)
        {
            float sourceX = (float)source.X;
            float sourceY = (float)source.Y;

            sourceX.AssertWithinEpsilon(target.X, _epsilon);
            sourceY.AssertWithinEpsilon(target.Y, _epsilon);
        }
    }
}