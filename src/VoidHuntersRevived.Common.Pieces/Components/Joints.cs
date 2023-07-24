using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Joints : IEntityComponent, IDisposable
    {
        public required Joint Child { get; init; }
        public required NativeDynamicArrayCast<Joint> Parents { get; init; }

        public void Dispose()
        {
            this.Parents.Dispose();
        }

        public static Joints Polygon(int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            Joint child = default;
            NativeDynamicArrayCast<Joint> parents = new NativeDynamicArrayCast<Joint>((uint)sides, Allocator.Persistent);

            for (int i = 0; i < vertexAngles.Length; i++)
            {
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[(i + 1) % vertexAngles.Length].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                if (i == 0)
                {
                    child = new Joint(byte.MaxValue, new Location(center, vertexAngles[i].Angle));
                }
                else
                {
                    parents.Set(i - 1, new Joint((byte)i, new Location(center, vertexAngles[i].Angle)));
                }
            }

            return new Joints()
            {
                Child = child,
                Parents = parents
            };
        }
    }
}
