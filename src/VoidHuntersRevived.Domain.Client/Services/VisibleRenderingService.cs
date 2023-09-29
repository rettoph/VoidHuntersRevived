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
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Domain.Client.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Domain.Client.Services
{
    internal sealed class VisibleRenderingService : IVisibleRenderingService
    {
        private readonly short[] _indexBuffer;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _tracePrimitiveBatch;
        private readonly IResourceProvider _resources;
        private readonly IScreen _screen;
        private readonly PrimitiveBatch<VertexVisible, VisibleEffect> _primitiveBatch;

        public VisibleRenderingService(
            Camera2D camera,
            PrimitiveBatch<VertexPositionColor> tracePrimitiveBatch,
            PrimitiveBatch<VertexVisible, VisibleEffect> fillPrimitiveBatch,
            IResourceProvider resources, 
            IScreen screen)
        {
            _resources = resources;
            _tracePrimitiveBatch = tracePrimitiveBatch;
            _primitiveBatch = fillPrimitiveBatch;
            _camera = camera;
            _screen = screen;
            _indexBuffer = new short[4];
        }

        public void Begin(Color primaryColor, Color secondaryColor)
        {
            _primitiveBatch.BlendState = BlendState.NonPremultiplied;
            _primitiveBatch.Begin(_camera);
            _primitiveBatch.Effect.PrimaryColor = primaryColor;
            _primitiveBatch.Effect.SecondaryColor = secondaryColor;

            float scale = 100 / (-_camera.Zoom - 100) + 1;
            _primitiveBatch.Effect.TraceScale = 0f;
            _primitiveBatch.Effect.TraceDiffusionScale = 0;
        }

        public void End()
        {
            _primitiveBatch.End();
        }

        public void Draw(in Visible visible, ref Matrix transformation)
        {
            for (int i = 0; i < visible.Fill.count; i++)
            {
                this.FillShape(in visible.Fill[i], ref transformation);
            }

            for (int i = 0; i < visible.TraceVertices.count; i++)
            {
                this.Trace(in visible.TraceVertices[i], ref transformation);
            }
        }

        private void Trace(in Shape shape, ref Matrix transformation)
        {
            _primitiveBatch.EnsureCapacity(4);

            // First Pass
            ref VertexVisible v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            Vector2.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);
            v1.Outer = true;

            ref VertexVisible v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
            Vector2.Transform(ref shape.Vertices[1], ref transformation, out v2.Position);
            v2.Outer = true;

            ref VertexVisible v3 = ref _primitiveBatch.NextVertex(out _indexBuffer[2]);
            Vector2.Transform(ref shape.Vertices[2], ref transformation, out v3.Position);
            v3.Outer = false;

            ref VertexVisible v4 = ref _primitiveBatch.NextVertex(out _indexBuffer[3]);
            Vector2.Transform(ref shape.Vertices[3], ref transformation, out v4.Position);
            v4.Outer = false;

            _primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            _primitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
            _primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);

            _primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            _primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);
            _primitiveBatch.AddTriangleIndex(in _indexBuffer[3]);
        }

        private void FillShape(in Shape shape, ref Matrix transformation)
        {
            _primitiveBatch.EnsureCapacity(shape.Vertices.count);

            ref VertexVisible v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            Vector2.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);
            v1.Trace = false;

            ref VertexVisible v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
            Vector2.Transform(ref shape.Vertices[1], ref transformation, out v2.Position);
            v2.Trace = false;

            for (int i = 2; i < shape.Vertices.count; i++)
            {
                ref VertexVisible v3 = ref _primitiveBatch.NextVertex(out _indexBuffer[2]);
                Vector2.Transform(ref shape.Vertices[i], ref transformation, out v3.Position);
                v3.Trace = false;

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);

                _indexBuffer[1] = _indexBuffer[2];
            }
        }
    }
}
