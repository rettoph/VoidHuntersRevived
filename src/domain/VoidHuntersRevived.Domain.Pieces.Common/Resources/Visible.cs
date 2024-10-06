using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Helpers;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;
using VoidHuntersRevived.Domain.Pieces.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces.Common.Resources
{
    public class Visible
    {
        private static readonly float TraceThickness = 1f;
        private static readonly Matrix OuterScaleMatrix = Matrix.CreateScale(0.1f);
        private static readonly Matrix InnerScaleMatrix = Matrix.CreateScale(-0.1f);

        private Shape[] _trace = [];

        public required PrimitiveContext Primitive { get; init; }

        public required Shape[] Fill { get; init; }
        public required Shape[] Trace
        {
            get => _trace;
            init
            {
                _trace = value;

                List<Shape> traceVertices = new(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    traceVertices.Add(BuildTraceVertices(ref value[i]));
                }

                this.TraceVertices = [.. traceVertices];
            }
        }

        public Shape[] TraceVertices { get; private set; } = [];

        private Shape BuildTraceVertices(ref Shape shape)
        {
            List<Vector2> vertices = [];

            for (int i = 0; i < shape.Vertices.Length; i++)
            {
                PopulateTraceVertices(
                    vertices: ref vertices,
                    p1: TryGetVertex(shape, i - 1),
                    vertex: TryGetVertex(shape, i) ?? throw new Exception(),
                    p2: TryGetVertex(shape, i + 1));
            }

            return new Shape()
            {
                Vertices = [.. vertices]
            };
        }

        private void PopulateTraceVertices(ref List<Vector2> vertices, Vector2? p1, Vector2 vertex, Vector2? p2)
        {
            p1 ??= p2;
            p2 ??= p1;

            // For reference, see
            // https://www.desmos.com/calculator/dxdt9k1usk
            vertices.Add(CalculateInner(p1!.Value, vertex, p2!.Value));
            vertices.Add(CalculateEdge(p2!.Value, vertex, p1!.Value));
            vertices.Add(CalculateCorner(p1!.Value, vertex, p2!.Value));
            vertices.Add(CalculateEdge(p1!.Value, vertex, p2!.Value));
            vertices.Add(vertex);
        }

        public static Visible Polygon(int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            throw new NotImplementedException();
            // return new Visible()
            // {
            //     Fill = new[]
            //     {
            //         new Shape()
            //         {
            //             Vertices = vertexAngles.Select(x => x.XnaVertex).ToNativeDynamicArray()
            //         }
            //     }.ToNativeDynamicArray(),
            //     Trace = new[]
            //     {
            //         new Shape()
            //         {
            //             Vertices = vertexAngles.Select(x => x.XnaVertex).Concat(vertexAngles.First().XnaVertex.Yield()).ToNativeDynamicArray()
            //         }
            //     }.ToNativeDynamicArray()
            // };
        }

        private static Vector2? TryGetVertex(Shape shape, int index)
        {
            bool wrap = shape.Vertices[shape.Vertices.Length - 1] == shape.Vertices[0];
            if (index < 0 && wrap)
            {
                index = shape.Vertices.Length + index - 1;
                return shape.Vertices[index];
            }

            if (index >= shape.Vertices.Length && wrap)
            {
                index = index % shape.Vertices.Length + 1;
                return shape.Vertices[index];
            }

            if (index < shape.Vertices.Length)
            {
                return shape.Vertices[index];
            }

            return null;
        }

        private static float AAS(float a1)
        {
            return a1 == 0 ? 0 : TraceThickness * MathF.Sin(MathHelper.PiOver2) / MathF.Sin(a1);
        }


        private Vector2 CalculateCorner(Vector2 p1, Vector2 vertex, Vector2 p2)
        {
            float angle = vertex.Angle(p1, p2);
            float gridAngle = vertex.Angle(p1);
            return vertex + Vector2Helper.FromPolar(gridAngle + angle / 2 + (angle < 0 ? MathHelper.Pi : 0), TraceThickness);
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

            return vertex + Vector2Helper.FromPolar(gridAngle + angle / 2, MathF.Abs(AAS(angle / 2)));
        }
    }
}
