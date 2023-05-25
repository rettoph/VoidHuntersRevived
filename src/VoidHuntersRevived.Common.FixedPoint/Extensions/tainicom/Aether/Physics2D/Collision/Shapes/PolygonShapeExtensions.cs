using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common;

namespace tainicom.Aether.Physics2D.Collision.Shapes
{
    public static class PolygonShapeExtensions
    {
        public static PolygonShape Clone(this PolygonShape shape, ref FixMatrix transformation)
        {
            PolygonShape clone = (PolygonShape)shape.Clone();

            clone.Vertices.Transform(ref transformation);

            clone.Vertices = clone.Vertices;

            return clone;
        }
    }
}
