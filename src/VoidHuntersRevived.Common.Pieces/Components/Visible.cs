using Guppy.MonoGame.Helpers;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [PolymorphicJsonType(nameof(Visible))]
    public struct Visible : IEntityComponent, IDisposable, IPieceComponent
    {
        private static Vector2 UnitX = Vector2.UnitX * 0.075f;
        private static readonly Matrix OuterScaleMatrix = Matrix.CreateScale(0.1f);
        private static readonly Matrix InnerScaleMatrix = Matrix.CreateScale(-0.1f);

        private NativeDynamicArrayCast<Shape> _trace;

        public required NativeDynamicArrayCast<Shape> Fill { get; init; }
        public required NativeDynamicArrayCast<Shape> Trace
        {
            get => _trace;
            init
            {
                _trace = value;
                int traceVertexShapeCount = 0;
                int mostVertices = 0;
                Shape shape;

                for (int i = 0; i < value.count; i++)
                {
                    shape = value[i];
                    traceVertexShapeCount += shape.Vertices.count - 1;
                    mostVertices = Math.Max(mostVertices, shape.Vertices.count);
                }

                NativeDynamicArrayCast<Shape> traceVertices = new NativeDynamicArrayCast<Shape>((uint)traceVertexShapeCount, Allocator.Persistent);
                int traceVerticesIndex = 0;

                for (int i = 0; i < value.count; i++)
                {
                    this.PopulateTraceVertices(ref traceVertices, ref traceVerticesIndex, ref value[i]);
                }
                

                this.TraceVertices = traceVertices;
            }
        }

        public NativeDynamicArrayCast<Shape> TraceVertices { get; private set; }

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

            for (int i = 0; i < this.TraceVertices.count; i++)
            {
                this.TraceVertices[i].Dispose();
            }

            this.Fill.Dispose();
            this.Trace.Dispose();
            this.TraceVertices.Dispose();
        }

        private void PopulateTraceVertices(ref NativeDynamicArrayCast<Shape> traceVertices, ref int traceVerticesIndex, ref Shape shape)
        {
            for (int i = 0; i < shape.Vertices.count; i++)
            {
                traceVertices.Set(traceVerticesIndex++, this.ConstructTraceSegment(
                    segStart: TryGetVertex(ref shape, i) ?? throw new Exception(),
                    segEnd: TryGetVertex(ref shape, i+1) ?? throw new Exception(),
                    next: TryGetVertex(ref shape, i+2),
                    prev: TryGetVertex(ref shape, i-1)));
            }
        }

        private Shape ConstructTraceSegment(Vector2 segStart, Vector2 segEnd, Vector2? next, Vector2? prev)
        {
            next ??= segStart;
            prev ??= segEnd;

            float segAngleStart = segStart.Angle(prev.Value, segEnd);
            float segAngleEnd = segEnd.Angle(segStart, next.Value);

            float gridAngleStart = segStart.Angle(prev.Value);
            float gridAngleEnd = segEnd.Angle(next.Value);

            return default;
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

        private static Vector2? TryGetVertex(ref Shape shape, int index)
        {
            bool wrap = shape.Vertices[shape.Vertices.count - 1] == shape.Vertices[0];
            if (index < 0 && wrap)
            {
                return shape.Vertices[shape.Vertices.count + index - 1];
            }

            if(index >= shape.Vertices.count && wrap)
            {
                return shape.Vertices[index % shape.Vertices.count];
            }

            if (index < shape.Vertices.count)
            {
                return shape.Vertices[index];
            }

            return null;
        }
    }
}
