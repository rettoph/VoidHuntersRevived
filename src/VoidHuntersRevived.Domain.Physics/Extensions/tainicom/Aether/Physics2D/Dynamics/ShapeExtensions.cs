using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using AetherVertices = tainicom.Aether.Physics2D.Common.Vertices;

namespace VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics
{
    public static class ShapeExtensions
    {
        public static Shape ToShape(this Polygon polygon)
        {
            AetherVertices vertices = new AetherVertices(polygon.Vertices.Select(x => x.AsAetherVector2()));
            vertices = GiftWrap.GetConvexHull(vertices);

            return new PolygonShape(vertices, polygon.Density);
        }
    }
}
