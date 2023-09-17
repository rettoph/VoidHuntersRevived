using Guppy.MonoGame;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class VisibleRenderingService : IVisibleRenderingService
    {
        private readonly short[] _indexBuffer;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly IResourceProvider _resources;
        private readonly IScreen _screen;

        public VisibleRenderingService(Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch, IResourceProvider resources, IScreen screen)
        {
            _resources = resources;
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _screen = screen;
            _indexBuffer = new short[3];
        }

        public void BeginFill()
        {
            _primitiveBatch.BlendState = BlendState.NonPremultiplied;
            _primitiveBatch.Begin(_camera);
        }

        public void BeginTrace()
        {
            _primitiveBatch.BlendState = BlendState.Additive;
            _primitiveBatch.Begin(_screen.Camera);
        }

        public void End()
        {
            _primitiveBatch.End();
        }

        public void Fill(in Visible visible, ref Matrix transformation, in Color color)
        {
            for (int i = 0; i < visible.Fill.count; i++)
            {
                this.FillShape(in visible.Fill[i], ref transformation, color);
            }
        }

        public void Trace(in Visible visible, ref Matrix transformation, in Color color)
        {
            for (int i = 0; i < visible.Trace.count; i++)
            {
                this.TracePath(in visible.Trace[i], ref transformation, color);
            }
        }

        private void FillShape(in Shape shape, ref Matrix transformation, Color color)
        {
            _primitiveBatch.EnsureCapacity(shape.Vertices.count);

            ref VertexPositionColor v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            v1.Color = color;
            Vector3.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);

            ref VertexPositionColor v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
            v2.Color = color;
            Vector3.Transform(ref shape.Vertices[1], ref transformation, out v2.Position);


            for (int i = 2; i < shape.Vertices.count; i++)
            {
                ref VertexPositionColor v3 = ref _primitiveBatch.NextVertex(out _indexBuffer[2]);
                v3.Color = color;
                Vector3.Transform(ref shape.Vertices[i], ref transformation, out v3.Position);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);

                _indexBuffer[1] = _indexBuffer[2];
            }
        }

        private void TracePath(in Shape shape, ref Matrix transformation, Color color)
        {
            _primitiveBatch.EnsureCapacity(shape.Vertices.count);

            ref VertexPositionColor v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            v1.Color = color;
            Vector3.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);
            v1.Position = _camera.Project(v1.Position);

            for (int i = 1; i < shape.Vertices.count; i++)
            {
                ref VertexPositionColor v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
                v2.Color = color;
                Vector3.Transform(ref shape.Vertices[i], ref transformation, out v2.Position);
                v2.Position = _camera.Project(v2.Position);

                _primitiveBatch.AddLineIndex(in _indexBuffer[0]);
                _primitiveBatch.AddLineIndex(in _indexBuffer[1]);

                _indexBuffer[0] = _indexBuffer[1];
            }
        }
    }
}
