using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Visible : IEntityComponent, IDisposable
    {
        public required NativeDynamicArrayCast<Shape> Shapes { get; init; }
        public required NativeDynamicArrayCast<Shape> Paths { get; init; }

        public void Dispose()
        {
            for (int i = 0; i < this.Shapes.count; i++)
            {
                this.Shapes[i].Dispose();
            }

            for (int i = 0; i < this.Paths.count; i++)
            {
                this.Paths[i].Dispose();
            }

            this.Shapes.Dispose();
            this.Paths.Dispose();
        }

        public static Visible Polygon(int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new Visible()
            {
                Shapes = new[]
                {
                    new Shape()
                    {
                        Vertices = vertexAngles.Select(x => x.XnaVertex.ToVector3()).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray(),
                Paths = new[]
                {
                    new Shape()
                    {
                        Vertices = vertexAngles.Select(x => x.XnaVertex.ToVector3()).Concat(vertexAngles.First().XnaVertex.ToVector3().Yield()).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray()
            };
        }
    }
}
