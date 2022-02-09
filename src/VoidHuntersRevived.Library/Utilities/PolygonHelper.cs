using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts.Utilities;

namespace VoidHuntersRevived.Library.Utilities
{
    public static class PolygonHelper
    {
        private static IEnumerable<(Vector2 vertex, Single angle)> VertexAngleData(Int32 sides)
        {
            Single angle = (MathHelper.TwoPi / sides);
            Single currentAngle = 0;
            Vector2 position = Vector2.Zero;

            yield return (position, currentAngle);

            for (Int32 i = 1; i < sides; i++)
            {
                currentAngle = angle * i;
                position += new Vector2(
                    x: (Single)Math.Cos(currentAngle),
                    y: (Single)Math.Sin(currentAngle));

                yield return (position, currentAngle);
            }
        }

        public static Vertices GetVertices(Int32 sides)
        {
            return new Vertices(PolygonHelper.VertexAngleData(sides).Select(va => va.vertex));
        }

        public static Vector2[] GetPath(Int32 sides)
        {
            List<Vector2> vertices = new List<Vector2>();

            vertices.AddRange(PolygonHelper.VertexAngleData(sides).Select(va => va.vertex));
            vertices.Add(vertices.First());

            return vertices.ToArray();
        }

        public static ConnectionNodeContext[] GetConnectionNodes(Int32 sides)
        {
            return PolygonHelper.VertexAngleData(sides).Select(va =>
            {
                return new ConnectionNodeContext()
                {
                    Position = va.vertex - new Vector2(
                        x: (Single)Math.Cos(va.angle) * 0.5f,
                        y: (Single)Math.Sin(va.angle) * 0.5f),
                    Rotation = va.angle + MathHelper.PiOver2
                };
            }).ToArray();
        }

        public static Vector2 GetCenteroid(Int32 sides)
        {
            return PolygonHelper.VertexAngleData(sides)
                .Select(va => va.vertex)
                .Aggregate((v1, v2) => v1 + v2) / sides;
        }
    }
}
