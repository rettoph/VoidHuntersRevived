using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using AetherVertices = tainicom.Aether.Physics2D.Common.Vertices;

namespace VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics
{
    public static class ShapeExtensions
    {
        public static Shape ToShape(this Polygon polygon, FixMatrix transformation)
        {
            AetherVector2[] vertexes = new AetherVector2[polygon.Vertices.Length];
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                var original = polygon.Vertices[i];
                vertexes[i] = FixVector2.Transform(original, transformation).AsAetherVector2();
            }

            AetherVertices vertices = new(vertexes);
            vertices = GiftWrap.GetConvexHull(vertices);

            return new PolygonShape(vertices, polygon.Density);
        }
    }
}
