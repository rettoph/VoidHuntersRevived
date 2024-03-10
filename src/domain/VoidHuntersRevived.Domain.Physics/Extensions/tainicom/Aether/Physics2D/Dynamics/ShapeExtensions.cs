using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common;
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
            AetherVector2[] vertexes = new AetherVector2[polygon.Vertices.count];
            for (int i = 0; i < polygon.Vertices.count; i++)
            {
                var original = polygon.Vertices[i];
                vertexes[i] = FixVector2.Transform(original, transformation).AsAetherVector2();
            }

            if (vertexes[0] == vertexes[1] && vertexes[0] == vertexes[2])
            {

            }

            AetherVertices vertices = new AetherVertices(vertexes);
            vertices = GiftWrap.GetConvexHull(vertices);

            return new PolygonShape(vertices, polygon.Density);
        }
    }
}
