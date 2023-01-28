using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Helpers
{
    public static class PolygonHelper
    {
        public readonly struct VertexAngle
        {
            public readonly Vector2 Vertex;
            public readonly float Angle;

            public VertexAngle(Vector2 vertex, float angle)
            {
                this.Vertex = vertex;
                this.Angle = angle;
            }
        }

        public static IEnumerable<VertexAngle> CalculateVertexAngles(int sides)
        {
            float interval = MathHelper.TwoPi / sides;
            float angle = 0;
            Vector2 position = Vector2.Zero;

            yield return new VertexAngle(position, angle);

            for(int i=1; i<sides; i++)
            {
                angle += interval;
                position += new Vector2()
                {
                    X = MathF.Cos(angle),
                    Y = MathF.Sin(angle)
                };

                yield return new VertexAngle(position, angle);
            }
        }
    }
}
