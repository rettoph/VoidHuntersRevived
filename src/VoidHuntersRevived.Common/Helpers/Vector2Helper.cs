using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Helpers
{
    public static class Vector2Helper
    {
        public static Vector2[] CreateCircle(float radius, int segments)
        {
            var vertices = new Vector2[segments + 1];
            var unit = Vector2.UnitX * radius;

            for (int i = 0; i < segments; i++)
            {
                var radians = (float)i / segments * MathHelper.TwoPi;
                var translation = Matrix.CreateRotationZ(radians);

                vertices[i] = Vector2.Transform(unit, translation);
            }

            vertices[segments] = vertices[0];

            return vertices;
        }

        public static Vector2 FromPolar(float radians, float length)
        {
            return new Vector2(
                x: MathF.Cos(radians) * length,
                y: MathF.Sin(radians) * length);
        }
    }
}
