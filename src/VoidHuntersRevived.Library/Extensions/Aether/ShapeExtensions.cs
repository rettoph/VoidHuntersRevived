using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;

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
            if(shape is PolygonShape polygon)
            {
                Vertices vertices = polygon.Vertices.Clone();
                vertices.Transform(ref transformation);

                return new PolygonShape(vertices, shape.Density);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unable to clone {shape.ShapeType} at this time.");
            }
        }
    }
}
