using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Visible : IEntityComponent
    {
        public required NativeDynamicArrayCast<Shape> Shapes { get; init; }
        public required NativeDynamicArrayCast<Shape> Paths { get; init; }

        public static Visible Polygon(Resource<Color> color, int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new Visible()
            {
                Shapes = new[]
                {
                    new Shape()
                    {
                        Color = color,
                        Vertices = vertexAngles.Select(x => x.XnaVertex.ToVector3()).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray(),
                Paths = new[]
                {
                    new Shape()
                    {
                        Color = color,
                        Vertices = vertexAngles.Select(x => x.XnaVertex.ToVector3()).Concat(vertexAngles.First().XnaVertex.ToVector3().Yield()).ToNativeDynamicArray()
                    }
                }.ToNativeDynamicArray()
            };
        }
    }
}
