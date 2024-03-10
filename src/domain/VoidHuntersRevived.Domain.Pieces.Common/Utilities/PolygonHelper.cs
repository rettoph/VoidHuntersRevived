using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Pieces.Common.Utilities
{
    public static class PolygonHelper
    {
        [StructLayout(LayoutKind.Explicit)]
        public readonly struct VertexAngle
        {
            [FieldOffset(0)]
            public readonly Vector2 XnaVertex;

            [FieldOffset(32)]
            public readonly FixVector2 FixedVertex;

            [FieldOffset(160)]
            public readonly Fix64 Angle;

            public VertexAngle(FixVector2 vertex, Fix64 angle)
            {
                this.XnaVertex = (Vector2)vertex;
                this.FixedVertex = vertex;
                this.Angle = angle;
            }
        }

        public static IEnumerable<VertexAngle> CalculateVertexAngles(int sides)
        {
            Fix64 interval = Fix64.PiTimes2 / (Fix64)sides;
            Fix64 angle = Fix64.Zero;
            FixVector2 position = FixVector2.Zero;

            yield return new VertexAngle(position, angle);

            for (int i = 1; i < sides; i++)
            {
                position += new FixVector2()
                {
                    X = Fix64.Cos(angle),
                    Y = Fix64.Sin(angle)
                };
                angle += interval;

                yield return new VertexAngle(position, angle);
            }
        }
    }
}
