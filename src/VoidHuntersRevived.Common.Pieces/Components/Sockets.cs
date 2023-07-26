using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Sockets : IEntityComponent, IDisposable
    {
        public required NativeDynamicArrayCast<Socket> Items { get; init; }

        public void Dispose()
        {
            this.Items.Dispose();
        }

        public static Sockets Polygon(in EntityId nodeId, int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            NativeDynamicArrayCast<Socket> items = new NativeDynamicArrayCast<Socket>((uint)sides, Allocator.Persistent);

            for (int i = 1; i < vertexAngles.Length; i++)
            {
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[(i + 1) % vertexAngles.Length].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                items.Set(i - 1, new Socket(nodeId, (byte)i, new Location(center, vertexAngles[i].Angle)));
            }

            return new Sockets()
            {
                Items = items
            };
        }
    }
}
