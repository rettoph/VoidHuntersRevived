using Guppy.MonoGame;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal partial class DrawSystem
    {
        private sealed class Renderer
        {
            private Vector2[][] _vertices;
            private Vector2[] _buffer;
            private readonly DrawConfiguration _configuration;
            private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
            private readonly Color _color;

            public Renderer(
                PrimitiveBatch<VertexPositionColor> primitiveBatch,
                IResourceProvider resources,
                DrawConfiguration configuration)
            {
                _configuration = configuration;
                _primitiveBatch = primitiveBatch;
                _vertices = _configuration.Shapes;
                _buffer = new Vector2[3];
                _color = resources.Get<Color>(_configuration.Color);
            }

            public void Render(Matrix transformation)
            {
                foreach(Vector2[] vertices in _vertices)
                {
                    // Pre-calculate & cache the first 2 vertices
                    _buffer[0] = Vector2.Transform(vertices[0], transformation);
                    _buffer[1] = Vector2.Transform(vertices[1], transformation);
                    for (int i = 2; i < vertices.Length; i++)
                    { // Iterate through each triangle to be drawn...

                        // Calculate a new vertice point
                        _buffer[2] = Vector2.Transform(vertices[i], transformation);

                        // Render the current triangle..
                        _primitiveBatch.DrawTriangle(_color, _buffer[0], _buffer[1], _buffer[2]);

                        // Move the old vertice down the buffer 1...
                        _buffer[1] = _buffer[2];
                    }
                }
            }
        }
    }
}
