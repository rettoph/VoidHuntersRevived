using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Rigid : IEntityComponent, IDisposable
    {
        public required FixVector2 Centeroid { get; init; }
        public required NativeDynamicArrayCast<Polygon> Shapes { get; init; }

        public void Dispose()
        {
            for(int i=0; i<this.Shapes.count; i++)
            {
                this.Shapes[i].Dispose();
            }

            this.Shapes.Dispose();
        }

        public static Rigid Polygon(Fix64 density, int sides)
        {
            FixVector2[] vertices = PolygonHelper.CalculateVertexAngles(sides).Select(x => x.FixedVertex).ToArray();
            Polygon shape = new Polygon(density, vertices);

            Rigid rigid = new Rigid()
            {
                Centeroid = shape.Centeroid,
                Shapes = new[]
                {
                    shape
                }.ToNativeDynamicArray()
            };

            return rigid;
        }
    }
}
