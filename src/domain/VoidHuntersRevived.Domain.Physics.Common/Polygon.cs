using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public readonly struct Polygon(Fix64 density, FixVector2[] vertices)
    {
        public readonly Fix64 Density = density;
        public readonly FixVector2[] Vertices = vertices;

        public FixVector2 Centeroid
        {
            get
            {
                FixVector2 value = FixVector2.Zero;

                for (int i = 0; i < this.Vertices.Length; i++)
                {
                    value += this.Vertices[i];
                }

                return value / (Fix64)this.Vertices.Length;
            }
        }
    }
}
