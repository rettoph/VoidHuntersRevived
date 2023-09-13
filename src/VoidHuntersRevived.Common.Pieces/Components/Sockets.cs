using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Attributes;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [AutoDisposeComponent<Location>(AutoDisposeScope.Type)]
    [AutoDisposeComponent<SocketId>(AutoDisposeScope.Instance)]
    public struct Sockets<T> : IEntityComponent, IDisposable
        where T : unmanaged
    {
        public required NativeDynamicArrayCast<T> Items { get; init; }

        public void Dispose()
        {
            this.Items.Dispose();
        }

        public static Sockets<Location> Polygon(int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            NativeDynamicArrayCast<Location> items = new NativeDynamicArrayCast<Location>((uint)sides - 1, Allocator.Persistent);

            for (int i = 1; i < vertexAngles.Length; i++)
            {
                int nextI = (i + 1) % vertexAngles.Length;
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[nextI].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                var location = new Location(center, vertexAngles[i].Angle - Fix64.PiOver2);
                items.Set(i - 1, location);
            }

            return new Sockets<Location>()
            {
                Items = items
            };
        }
    }
}
