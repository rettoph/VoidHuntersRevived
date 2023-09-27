using Guppy.MonoGame.Helpers;
using Guppy.Resources;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System.Drawing;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [PolymorphicJsonType(nameof(Visible))]
    public struct Visible : IEntityComponent, IDisposable, IPieceComponent
    {
        private static Vector2 UnitX = Vector2.UnitX * 0.1f;
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
            Span<Vector2> inners = stackalloc Vector2[shape.Vertices.count];
            Span<Vector2> outers = stackalloc Vector2[shape.Vertices.count];

            this.CalculateVertexOffsets(
                vertex: shape.Vertices[0], 
                next: shape.Vertices[1], 
                prev: shape.Vertices[shape.Vertices.count - 1] == shape.Vertices[0] ? shape.Vertices[shape.Vertices.count - 2] : null, 
                inner: out inners[0], 
                outer: out outers[0]);

            for (int i = 1; i < shape.Vertices.count - 1; i++)
            {
                this.CalculateVertexOffsets(
                    vertex: shape.Vertices[i],
                    next: shape.Vertices[i+1],
                    prev: shape.Vertices[i-1],
                    inner: out inners[i],
                    outer: out outers[i]);
            }


            this.CalculateVertexOffsets(
                vertex: shape.Vertices[shape.Vertices.count - 1],
                next: shape.Vertices[shape.Vertices.count - 1] == shape.Vertices[0] ? shape.Vertices[1] : null,
                prev: shape.Vertices[shape.Vertices.count - 2],
                inner: out inners[shape.Vertices.count - 1],
                outer: out outers[shape.Vertices.count - 1]);

            for (int i = 0; i < shape.Vertices.count - 1; i++)
            {
                NativeDynamicArrayCast<Vector2> shapeVertices = new NativeDynamicArrayCast<Vector2>(4, Allocator.Persistent);
                shapeVertices.Set(0, outers[i]);
                shapeVertices.Set(1, outers[i + 1]);
                shapeVertices.Set(2, inners[i + 1]);
                shapeVertices.Set(3, inners[i]);

                traceVertices.Set(traceVerticesIndex++, new Shape()
                {
                    Vertices = shapeVertices
                });
            }
        }

        private void CalculateVertexOffsets(Vector2 vertex, Vector2? next, Vector2? prev, out Vector2 inner, out Vector2 outer)
        {
            if(next is null && prev is null)
            {
                throw new ArgumentException();
            }

            float prevAngle = prev?.Angle(vertex) ?? next!.Value.Angle(vertex) + MathHelper.Pi;
            float nextAngle = next?.Angle(vertex) ?? prevAngle + MathHelper.Pi;
            float angle = (prevAngle - nextAngle) / 2;
            float targetAngle = prevAngle - angle;

            var triangle1 = TriangleHelper.Solve(A: angle, B: MathHelper.PiOver2, a: 1f);

            Matrix transformationInner = Matrix.CreateScale(triangle1.b) * Matrix.CreateRotationZ(targetAngle) * Matrix.CreateTranslation(vertex.X, vertex.Y, 0);
            Matrix transformationOuter = Matrix.CreateScale(triangle1.b) * Matrix.CreateRotationZ(targetAngle + MathHelper.Pi) * Matrix.CreateTranslation(vertex.X, vertex.Y, 0);

            Vector2.Transform(ref UnitX, ref transformationInner, out inner);
            Vector2.Transform(ref UnitX, ref transformationOuter, out outer);
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
