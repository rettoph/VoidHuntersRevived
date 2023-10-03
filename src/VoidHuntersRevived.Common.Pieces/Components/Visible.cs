using Guppy.MonoGame.Helpers;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Helpers;
using VoidHuntersRevived.Common.Pieces.Enums;
using VoidHuntersRevived.Common.Pieces.Utilities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [PolymorphicJsonType(nameof(Visible))]
    public struct Visible : IEntityComponent, IDisposable, IPieceComponent
    {
        private static float TraceThickness = 1f;
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

                NativeDynamicArrayCast<Shape> traceVertices = new NativeDynamicArrayCast<Shape>((uint)value.count, Allocator.Persistent);

                for (int i = 0; i < value.count; i++)
                {
                    traceVertices.Set(i, this.BuildTraceVertices(ref value[i]));
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

        private Shape BuildTraceVertices(ref Shape shape)
        {
            NativeDynamicArrayCast<Vector2> vertices = new NativeDynamicArrayCast<Vector2>((uint)shape.Vertices.count * 5, Allocator.Persistent);

            uint index = 0;
            for (int i = 0; i < shape.Vertices.count; i++)
            {
                this.PopulateTraceVertices(
                    vertices: ref vertices,
                    index: ref index,
                    p1: TryGetVertex(ref shape, i - 1),
                    vertex: TryGetVertex(ref shape, i) ?? throw new Exception(),
                    p2: TryGetVertex(ref shape, i + 1));
            }

            return new Shape()
            {
                Vertices = vertices
            };
        }

        private void PopulateTraceVertices(ref NativeDynamicArrayCast<Vector2> vertices, ref uint index, Vector2? p1, Vector2 vertex, Vector2? p2)
        {
            p1 ??= p2;
            p2 ??= p1;

            // For reference, see
            // https://www.desmos.com/calculator/dxdt9k1usk
            vertices.Set(index++, CalculateInner(p1!.Value, vertex, p2!.Value));
            vertices.Set(index++, CalculateEdge(p2!.Value, vertex, p1!.Value));
            vertices.Set(index++, CalculateCorner(p1!.Value, vertex, p2!.Value));
            vertices.Set(index++, CalculateEdge(p1!.Value, vertex, p2!.Value));
            vertices.Set(index++, vertex);
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
                index = shape.Vertices.count + index - 1;
                return shape.Vertices[index];
            }

            if(index >= shape.Vertices.count && wrap)
            {
                index = (index % shape.Vertices.count) + 1;
                return shape.Vertices[index];
            }

            if (index < shape.Vertices.count)
            {
                return shape.Vertices[index];
            }

            return null;
        }

        private static float AAS(float a1)
        {
            return a1 == 0 ? 0 : (TraceThickness * MathF.Sin(MathHelper.PiOver2)) / MathF.Sin(a1);
        }


        private Vector2 CalculateCorner(Vector2 p1, Vector2 vertex, Vector2 p2)
        {
            float angle = vertex.Angle(p1, p2);
            float gridAngle = vertex.Angle(p1);
            return vertex + Vector2Helper.FromPolar(gridAngle + (angle / 2) + (angle < 0 ? MathHelper.Pi : 0), TraceThickness);
        }

        private Vector2 CalculateEdge(Vector2 p1, Vector2 vertex, Vector2 p2)
        {
            float angle = vertex.Angle(p1, p2);
            float gridAngle = vertex.Angle(p1);

            return vertex + Vector2Helper.FromPolar(gridAngle + MathHelper.Pi, TraceThickness);
        }

        private Vector2 CalculateInner(Vector2 p1, Vector2 vertex, Vector2 p2)
        {
            float angle = vertex.Angle(p1, p2);
            float gridAngle = vertex.Angle(p1);

            return vertex + Vector2Helper.FromPolar(gridAngle + (angle / 2), MathF.Abs(AAS(angle / 2)));

        }
    }
}
