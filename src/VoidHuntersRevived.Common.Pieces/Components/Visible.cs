using Guppy.Common.Extensions.System;
using Guppy.MonoGame.Helpers;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions.System;
using VoidHuntersRevived.Common.Helpers;
using VoidHuntersRevived.Common.Pieces.Utilities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    [PolymorphicJsonType(nameof(Visible))]
    public struct Visible : IEntityComponent, IDisposable, IPieceComponent
    {
        private static float TraceThickness = 0.12f;
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
            for (int i = 0; i < shape.Vertices.count - 1; i++)
            {
                traceVertices.Set(traceVerticesIndex++, this.ConstructTraceSegment(
                    segStart: TryGetVertex(ref shape, i) ?? throw new Exception(),
                    segEnd: TryGetVertex(ref shape, i+1) ?? throw new Exception(),
                    next: TryGetVertex(ref shape, i+2),
                    prev: TryGetVertex(ref shape, i-1)));
            }
        }

        /// <summary>
        /// https://www.desmos.com/calculator/fu01mebdae
        /// </summary>
        /// <param name="segStart"></param>
        /// <param name="segEnd"></param>
        /// <param name="next"></param>
        /// <param name="prev"></param>
        /// <returns></returns>
        private Shape ConstructTraceSegment(Vector2 segStart, Vector2 segEnd, Vector2? next, Vector2? prev)
        {
            next ??= segStart;
            prev ??= segEnd;

            float segAngleStart = segStart.Angle(prev.Value, segEnd);
            float segAngleEnd = segEnd.Angle(next.Value, segStart);

            float gridAngleStartPrev = segStart.Angle(prev.Value);
            float gridAngleEndNext = segEnd.Angle(next.Value);
            float gridAngleStartEnd = segStart.Angle(segEnd);
            float gridAngleEndStart = segEnd.Angle(segStart);

            Vector2[] vertices = new Vector2[6];
            vertices[0] = segStart + Vector2Helper.FromPolar(gridAngleStartPrev + (segAngleStart / 2) + (segAngleStart < 0 ? MathHelper.Pi : 0), TraceThickness);
            vertices[1] = segStart + Vector2Helper.FromPolar(gridAngleStartPrev + (segAngleStart / 2) + (segAngleStart < 0 ? 0 : MathHelper.Pi), MathF.Abs(AAS(segAngleStart / 2)));
            vertices[2] = segStart + Vector2Helper.FromPolar(gridAngleStartEnd + (segAngleStart < 0 ? -MathHelper.PiOver2 : MathHelper.PiOver2), TraceThickness);
            vertices[3] = segEnd + Vector2Helper.FromPolar(gridAngleEndStart + (segAngleEnd < 0 ? -MathHelper.PiOver2 : MathHelper.PiOver2), TraceThickness);
            vertices[4] = segEnd + Vector2Helper.FromPolar(gridAngleEndNext + (segAngleEnd / 2) + (segAngleEnd < 0 ? MathHelper.Pi :0), MathF.Abs(AAS(segAngleEnd / 2)));
            vertices[5] = segEnd + Vector2Helper.FromPolar(gridAngleEndNext + (segAngleEnd / 2) + (segAngleEnd < 0 ? 0 : MathHelper.Pi), TraceThickness);

            Vector2[] convex = GiftWrap.GetConvexHull(vertices);

            return new Shape() { Vertices = convex.ToNativeDynamicArray() };
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

        private static float PiOrZero(float radians)
        {
            return radians >= MathHelper.Pi ? MathHelper.Pi : 0;
        }

        private static float IPiOrZero(float radians)
        {
            return radians >= MathHelper.Pi ? 0 : MathHelper.Pi;
        }

        private static float Sign(float radians, float value)
        {
            return (radians >= MathHelper.Pi ? 1f : -1f) * value;
        }

        private static float AAS(float a1)
        {
            return a1 == 0 ? 0 : (TraceThickness * MathF.Sin(MathHelper.PiOver2)) / MathF.Sin(a1);
        }
    }
}
