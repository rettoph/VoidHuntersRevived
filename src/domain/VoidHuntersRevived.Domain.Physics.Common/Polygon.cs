using Svelto.Common;
using Svelto.DataStructures;
using VoidHuntersRevived.Common.Extensions.Svelto;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public struct Polygon(Fix64 density, NativeDynamicArrayCast<FixVector2> vertices) : IDisposable
    {
        public readonly Fix64 Density = density;
        public readonly NativeDynamicArrayCast<FixVector2> Vertices = vertices;

        public FixVector2 Centeroid
        {
            get
            {
                FixVector2 value = FixVector2.Zero;

                for (int i = 0; i < this.Vertices.count; i++)
                {
                    value += this.Vertices[i];
                }

                return value / (Fix64)this.Vertices.count;
            }
        }

        public Polygon(Fix64 density, params FixVector2[] vertices) : this(density, vertices.ToNativeDynamicArray())
        {
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
        }

        public Polygon Clone()
        {
            return new Polygon(
                density: this.Density,
                vertices: this.Vertices.Clone(Allocator.Persistent));
        }
    }
}
