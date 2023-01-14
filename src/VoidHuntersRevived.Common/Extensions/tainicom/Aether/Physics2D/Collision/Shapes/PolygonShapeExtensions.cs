using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tainicom.Aether.Physics2D.Collision.Shapes
{
    public static class PolygonShapeExtensions
    {
        public static PolygonShape Clone(this PolygonShape shape, ref Matrix transformation)
        {
            var clone = (PolygonShape)shape.Clone();

            clone.Vertices.Transform(ref transformation);

            return clone;
        }
    }
}
