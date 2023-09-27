using Guppy.Resources;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [PolymorphicJsonType(nameof(Visible))]
    public struct Visible : IEntityComponent, IDisposable, IPieceComponent
    {
        public required NativeDynamicArrayCast<Shape> Fill { get; init; }
        public required NativeDynamicArrayCast<Shape> Trace { get; init; }

        public void Dispose()
        {
            for (int i = 0; i < this.Fill.count; i++)
            {
                this.Fill[i].Dispose();
            }

            for (int i = 0; i < this.Trace.count; i++)
            {
                this.Trace[i].Dispose();
            }

            this.Fill.Dispose();
            this.Trace.Dispose();
        }

        public static Visible Polygon(int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new Visible()
            {
                Fill = new[]
                {
                    new Shape()
                    {
                        Vertices = vertexAngles.Select(x => x.XnaVertex).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray(),
                Trace = new[]
                {
                    new Shape()
                    {
                        Vertices = vertexAngles.Select(x => x.XnaVertex).Concat(vertexAngles.First().XnaVertex.Yield()).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray()
            };
        }
    }
}
