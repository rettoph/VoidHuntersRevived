using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint
{
    public class FixVector2Tests
    {
        [Theory]
        [InlineData(420, 69, -1.337, -123, 45)]
        [InlineData(-420, 69, 1.337, 123, -45)]
        [InlineData(420, -69, -1.337, -123, -45)]
        [InlineData(-420, -69, 1.337, 123, -45)]
        [InlineData(123, 45, 0, -123, 45)]
        [InlineData(123, 45, MathF.PI, -123, -45)]
        [InlineData(123, 45, MathHelper.TwoPi, 123, 45)]
        public void FixVector2_Transform_Test(float tX, float tY, float tRot, float vX, float vY) => FixVector2.Transform(
                position: new FixVector2((Fix64)vX, (Fix64)vY),
                transform: new FixTransform2D((Fix64)tX, (Fix64)tY, (Fix64)tRot)
            ).AssetEqualTo(
                target: Vector2.Transform(
                    position: new Vector2(vX, vY),
                    matrix: Matrix.CreateRotationZ(tRot) * Matrix.CreateTranslation(tX, tY, 0)
                )
            );
    }
}