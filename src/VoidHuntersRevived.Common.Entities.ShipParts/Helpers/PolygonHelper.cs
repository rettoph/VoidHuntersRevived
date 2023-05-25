using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using FloatingVector2 = Microsoft.Xna.Framework.Vector2;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Helpers
{
    public static class PolygonHelper
    {
        public readonly struct VertexAngle
        {
            public readonly FloatingVector2 Floating;
            public readonly AetherVector2 Fixed;
            public readonly Fix64 Angle;

            public VertexAngle(AetherVector2 vertex, Fix64 angle)
            {
                this.Floating = vertex.ToXnaVector2();
                this.Fixed = vertex;
                this.Angle = angle;
            }
        }

        public static IEnumerable<VertexAngle> CalculateVertexAngles(int sides)
        {
            Fix64 interval = Fix64.PiTimes2 / (Fix64)sides;
            Fix64 angle = Fix64.Zero;
            AetherVector2 position = AetherVector2.Zero;

            yield return new VertexAngle(position, angle);

            for(int i=1; i<sides; i++)
            {
                angle += interval;
                position += new AetherVector2()
                {
                    X = Fix64.Cos(angle),
                    Y = Fix64.Sin(angle)
                };

                yield return new VertexAngle(position, angle);
            }
        }
    }
}
