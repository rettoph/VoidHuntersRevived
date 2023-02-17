using Guppy.MonoGame.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Editor.Services
{
    internal partial class VerticesBuilder
    {
        public class Outline : IPrimitiveShape<VertexPositionColor>
        {
            public readonly List<Vector2> Vertices;

            public Outline() : this(new List<Vector2>())
            {

            }
            public Outline(List<Vector2> vertices)
            {
                this.Vertices = vertices;
            }

            int IPrimitiveShape<VertexPositionColor>.Length => this.Vertices.Count;

            public void Transform(int index, in Color color, ref Matrix transformation, out VertexPositionColor output)
            {
                if (this.Vertices.Count == 0)
                {
                    output = default;
                    return;
                }

                output.Color = color;

                var transformed = Vector2.Transform(this.Vertices[index], transformation);
                output.Position.X = transformed.X;
                output.Position.Y = transformed.Y;
                output.Position.Z = 0;
            }

            public void Clean(IEnumerable<Vector2> vertices)
            {
                this.Vertices.Clear();
                this.Vertices.AddRange(vertices);
            }
        }
    }
}
