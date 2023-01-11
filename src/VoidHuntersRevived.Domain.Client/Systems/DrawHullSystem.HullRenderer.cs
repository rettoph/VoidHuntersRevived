using Guppy.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal partial class DrawHullSystem
    {
        private sealed class HullRenderer
        {
            private Vector2[][] _vertices;
            private Vector2[] _buffer;
            private readonly Hull _hull;
            private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;

            public HullRenderer(
                PrimitiveBatch<VertexPositionColor> primitiveBatch,
                Hull hull)
            {
                _hull = hull;
                _primitiveBatch = primitiveBatch;
                _vertices = _hull.Shapes.Select(x => x.Polygon.Vertices.ToArray()).ToArray();
                _buffer = new Vector2[3];
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
                        _primitiveBatch.DrawTriangle(Color.Red, _buffer[0], _buffer[1], _buffer[2]);

                        // Move the old vertice down the buffer 1...
                        _buffer[1] = _buffer[2];
                    }
                }
            }
        }
    }
}
