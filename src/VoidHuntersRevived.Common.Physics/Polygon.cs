using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Extensions.System;

namespace VoidHuntersRevived.Common.Physics
{
    public struct Polygon : IDisposable
    {
        public NativeDynamicArrayCast<FixVector2> Vertices;
        public Fix64 Density;

        public Polygon(Fix64 density, params FixVector2[] vertices) : this(density, vertices.ToNativeDynamicArray())
        {
        }
        public Polygon(Fix64 density, NativeDynamicArrayCast<FixVector2> vertices)
        {
            this.Density = density;
            this.Vertices = vertices;
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
        }
    }
}
