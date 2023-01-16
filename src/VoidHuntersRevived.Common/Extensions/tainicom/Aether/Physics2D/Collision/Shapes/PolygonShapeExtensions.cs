using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common.ConvexHull;

namespace tainicom.Aether.Physics2D.Collision.Shapes
{
    public static class PolygonShapeExtensions
    {
        public static PolygonShape Clone(this PolygonShape shape, ref Matrix transformation)
        {
            var clone = (PolygonShape)shape.Clone();

            clone.Vertices.Transform(ref transformation);

            clone.Vertices = clone.Vertices;

            return clone;
        }
    }
}
