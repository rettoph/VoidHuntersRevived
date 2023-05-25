using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace tainicom.Aether.Physics2D.Collision.Shapes
{
    public static class ShapeExtensions
    {
        public static Shape Clone(this Shape shape, ref FixedMatrix transformation)
        {
            return shape.ShapeType switch
            {
                ShapeType.Polygon => ((PolygonShape)shape).Clone(ref transformation),
                _ => throw new NotImplementedException()
            };
        }
    }
}
