using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint
{
    public class FixTransform2DTests
    {
        [Theory]
        [InlineData(420, 69, -1.337)]
        [InlineData(-420, 69, 1.337)]
        [InlineData(420, -69, -1.337)]
        [InlineData(-420, -69, 1.337)]
        [InlineData(123, 45, 0)]
        [InlineData(123, 45, MathF.PI)]
        [InlineData(123, 45, MathHelper.TwoPi)]
        public void FixTransform2D_Test(float x, float y, float rotation) => new FixTransform2D((Fix64)x, (Fix64)y, (Fix64)rotation)
                .AssetEqualTo(
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(x, y, 0)
                );

        [Theory]
        [InlineData(420, 69, 1.337)]
        [InlineData(-420, 69, -1.337)]
        [InlineData(420, -69, 1.337)]
        [InlineData(-420, -69, -1.337)]
        [InlineData(123, 45, 0)]
        [InlineData(123, 45, MathF.PI)]
        [InlineData(123, 45, MathHelper.TwoPi)]
        public void FixTransform2D_Invert_Test(float x, float y, float rotation) => FixTransform2D.Invert(new((Fix64)x, (Fix64)y, (Fix64)rotation))
                .AssetEqualTo(Matrix.Invert(
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(x, y, 0)
                ));

        [Theory]
        [InlineData(420, 69, -1.337, -123, 45)]
        [InlineData(-420, 69, 1.337, 123, -45)]
        [InlineData(420, -69, -1.337, -123, -45)]
        [InlineData(-420, -69, 1.337, 123, -45)]
        [InlineData(123, 45, 0, -123, 45)]
        [InlineData(123, 45, MathF.PI, -123, -45)]
        [InlineData(123, 45, MathHelper.TwoPi, 123, 45)]
        public void FixTransform2D_Multiply_Test(float x, float y, float rotation, float tranX, float tranY) => (new FixTransform2D((Fix64)x, (Fix64)y, (Fix64)rotation) * new FixVector2(tranX, tranY))
                .AssetEqualTo(
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(x, y, 0) *
                    Matrix.CreateRotationZ(0f) * Matrix.CreateTranslation(tranX, tranY, 0)
                );
    }
}