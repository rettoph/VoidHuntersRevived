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
        private static float TraceThickness = 0.1f;
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

                NativeDynamicArrayCast<TraceVertices> traceVertices = new NativeDynamicArrayCast<TraceVertices>((uint)traceVertexShapeCount, Allocator.Persistent);
                int traceVerticesIndex = 0;

                for (int i = 0; i < value.count; i++)
                {
                    this.PopulateTraceVertices(ref traceVertices, ref traceVerticesIndex, ref value[i]);
                }
                

                this.TraceVertices = traceVertices;
            }
        }

        public NativeDynamicArrayCast<TraceVertices> TraceVertices { get; private set; }

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

        private void PopulateTraceVertices(ref NativeDynamicArrayCast<TraceVertices> traceVertices, ref int traceVerticesIndex, ref Shape shape)
        {
            for (int i = 0; i < shape.Vertices.count - 1; i++)
            {
                traceVertices.Set(traceVerticesIndex++, this.ConstructTraceSegment(
                    segStart: TryGetVertex(ref shape, i) ?? throw new Exception(),
                    segEnd: TryGetVertex(ref shape, i+1) ?? throw new Exception(),
                    next: TryGetVertex(ref shape, i+2),
                    prev: TryGetVertex(ref shape, i-1)));
            }
        }

        private TraceVertices ConstructTraceSegment(Vector2 segStart, Vector2 segEnd, Vector2? next, Vector2? prev)
        {
            next ??= segStart;
            prev ??= segEnd;

            float segAngleStart = segStart.Angle(prev.Value, segEnd);
            float segAngleEnd = segEnd.Angle(next.Value, segStart);

            float gridAngleStartPrev = segStart.Angle(prev.Value);
            float gridAngleEndNext = segEnd.Angle(next.Value);
            float gridAngleStartEnd = segStart.Angle(segEnd);
            float gridAngleEndStart = segEnd.Angle(segStart);

            // For reference, see
            // https://www.desmos.com/calculator/px5jojwo7f
            Vector2[] vertices = new Vector2[8];

            // start
            vertices[0] = segStart;

            // start corner
            vertices[1] = CalculateCorner(prev.Value, segStart, segEnd);

            // start edge
            vertices[2] = CalculateEdge(prev.Value, segStart, segEnd);

            // start inner
            vertices[3] = CalculateInner(prev.Value, segStart, segEnd);

            // end
            vertices[4] = segEnd;

            // end edge
            vertices[5] = CalculateEdge(next.Value, segEnd, segStart);

            // end corner
            vertices[6] = CalculateCorner(segStart, segEnd, next.Value);

            // end inner
            vertices[7] = CalculateInner(segStart, segEnd, next.Value);

            // If both angles are nexative or both angles are positive
            // that means the outer triangles are inverse
            // if they are opposite; the triangles are calculated in the right order
            // https://i.imgur.com/JNnG0MK.jpg
            TraceVertex[] traceVertices = new TraceVertex[8];
            if (segAngleStart * segAngleEnd < 0)
            { // Convex shape, no flip required
                traceVertices[0] = new TraceVertex(vertices[0], false);
                traceVertices[1] = new TraceVertex(vertices[1], true);
                traceVertices[2] = new TraceVertex(vertices[2], true);
                traceVertices[3] = new TraceVertex(vertices[3], true);
                traceVertices[4] = new TraceVertex(vertices[4], false);
                traceVertices[5] = new TraceVertex(vertices[5], true);
                traceVertices[6] = new TraceVertex(vertices[6], true);
                traceVertices[7] = new TraceVertex(vertices[7], true);
            }
            else 
            { // Concave shape, flip required
                // Swapping these vertices should fix
                // the angle mismatch
                Vector2 placeholder = vertices[4];
                vertices[4] = vertices[5];
                vertices[5] = placeholder;

                // This probably wont work. I need to make sure the vertices
                // are clockwise and in such an order that the inner/outer
                // bools are correct. It will probably need a double check.
                // Check out VisibleRenderingService.cs and Visible.fx
                throw new NotImplementedException();
            }

            return new TraceVertices() { 
                Items = traceVertices.ToNativeDynamicArray()
            };
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
