using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.Svelto;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Interfaces;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Sockets : IEntityComponent, IDisposable, IPieceComponent, ICloneableComponent<Sockets>, ICompositeBelongsTo<Body, Fixture, Sockets>
    {
        public required NativeDynamicArrayCast<Socket> Items { get; init; }

        public Sockets()
        {

        }

        public void Dispose() => this.Items.Dispose();

        public static Sockets Polygon(int sides)
        {
            PolygonHelper.VertexAngle[] vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();

            NativeDynamicArrayCast<Socket> items = new((uint)sides - 1, Allocator.Persistent);

            for (int i = 1; i < vertexAngles.Length; i++)
            {
                int nextI = (i + 1) % vertexAngles.Length;
                FixVector2 start = vertexAngles[i].FixedVertex;
                FixVector2 end = vertexAngles[nextI].FixedVertex;
                FixVector2 center = (start + end) / (Fix64)2;

                FixTransform2D transform = new(center, vertexAngles[i].Angle - Fix64.PiOver2);
                items.Set(i - 1, new Socket(transform));
            }

            return new Sockets()
            {
                Items = items
            };
        }

        public Sockets Clone() => new()
        {
            Items = this.Items.Clone(Allocator.Persistent)
        };
    }
}