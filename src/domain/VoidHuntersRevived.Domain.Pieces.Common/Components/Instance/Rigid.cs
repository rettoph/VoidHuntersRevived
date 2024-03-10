using Guppy.Resources.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    [PolymorphicJsonType<IPieceComponent>(nameof(Rigid))]
    public struct Rigid : IEntityComponent, IDisposable, IPieceComponent
    {
        public required FixVector2 Centeroid { get; init; }
        public required NativeDynamicArrayCast<Polygon> Shapes { get; init; }

        public void Dispose()
        {
            for (int i = 0; i < Shapes.count; i++)
            {
                Shapes[i].Dispose();
            }

            Shapes.Dispose();
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
