using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class ShapeExtensions
    {
        /// <summary>
        /// Clone the concrete shape & transform it.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="transformation"></param>
        /// <returns>A clone of the shape</returns>
        public static Shape Clone(this Shape shape, Matrix transformation)
        {
            Shape clone = shape.Clone();

            if(clone is PolygonShape polygon)
            {
                polygon.Vertices.Transform(ref transformation);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unable to clone {clone.ShapeType} at this time.");
            }

            return clone;
        }
    }
}
