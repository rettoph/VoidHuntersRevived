using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using AetherVertices = tainicom.Aether.Physics2D.Common.Vertices;

namespace VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics
{
    public static class ShapeExtensions
    {
        public static Shape ToShape(this Polygon polygon, FixMatrix transformation)
        {
            AetherVector2[] vertexes = new AetherVector2[polygon.Vertices.count];
            for(int i=0; i<polygon.Vertices.count; i++)
            {
                vertexes[i] = FixVector2.Transform(polygon.Vertices[i], transformation).AsAetherVector2();
            }

            AetherVertices vertices = new AetherVertices(vertexes);
            vertices = GiftWrap.GetConvexHull(vertices);

            return new PolygonShape(vertices, polygon.Density);
        }
    }
}
