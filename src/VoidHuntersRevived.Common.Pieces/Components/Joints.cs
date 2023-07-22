using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Joints : IEntityComponent, IDisposable
    {
        public required NativeDynamicArrayCast<Joint> Items { get; init; }

        public void Dispose()
        {
            this.Items.Dispose();
        }

        public static Joints Polygon(int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            Joints joints = new Joints()
            {
                Items = new NativeDynamicArrayCast<Joint>((uint)sides, Allocator.Persistent)
            };

            for (int i = 0; i < vertexAngles.Length; i++)
            {
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[(i + 1) % vertexAngles.Length].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                joints.Items.Set(i, new Joint((byte)i, new FixLocation(center, vertexAngles[i].Angle)));
            }

            return joints;
        }
    }
}
