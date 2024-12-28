using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint
{
    public class FixMatrix2DTests
    {
        [Theory]
        [InlineData(420, 69, 1.337)]
        [InlineData(123, 45, 0)]
        [InlineData(123, 45, MathF.PI)]
        [InlineData(123, 45, MathHelper.TwoPi)]
        public void FixMatrix2D_Test(float x, float y, float rotation)
        {
            new FixMatrix2D((Fix64)x, (Fix64)y, (Fix64)rotation)
                .AssetEqualTo(
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(x, y, 0)
                );
        }

        [Theory]
        [InlineData(420, 69, 1.337)]
        [InlineData(123, 45, 0)]
        [InlineData(123, 45, MathF.PI)]
        [InlineData(123, 45, MathHelper.TwoPi)]
        public void FixMatrix2D_Invert_Test(float x, float y, float rotation)
        {
            FixMatrix2D.Invert(new((Fix64)x, (Fix64)y, (Fix64)rotation))
                .AssetEqualTo(Matrix.Invert(
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(x, y, 0)
                ));
        }
    }
}