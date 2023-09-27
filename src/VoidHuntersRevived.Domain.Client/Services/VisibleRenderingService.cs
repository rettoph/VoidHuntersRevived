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
        private readonly PrimitiveBatch<VertexVisible, VisibleFillEffect> _fillPrimitiveBatch;

        public VisibleRenderingService(
            Camera2D camera,
            PrimitiveBatch<VertexPositionColor> tracePrimitiveBatch,
            PrimitiveBatch<VertexVisible, VisibleFillEffect> fillPrimitiveBatch,
            IResourceProvider resources, 
            IScreen screen)
        {
            _resources = resources;
            _tracePrimitiveBatch = tracePrimitiveBatch;
            _fillPrimitiveBatch = fillPrimitiveBatch;
            _camera = camera;
            _screen = screen;
            _indexBuffer = new short[3];
        }

        public void BeginFill(Color color)
        {
            _fillPrimitiveBatch.BlendState = BlendState.NonPremultiplied;
            _fillPrimitiveBatch.Begin(_camera);
            _fillPrimitiveBatch.Effect.Color = color;
        }

        public void BeginTrace(Color color)
        {
            _tracePrimitiveBatch.BlendState = BlendState.Additive;
            _tracePrimitiveBatch.Begin(_screen.Camera);
        }

        public void EndTrace()
        {
            _tracePrimitiveBatch.End();
        }

        public void EndFill()
        {
            _fillPrimitiveBatch.End();
        }

        public void Fill(in Visible visible, ref Matrix transformation)
        {
            for (int i = 0; i < visible.Fill.count; i++)
            {
                this.FillShape(in visible.Fill[i], ref transformation);
            }
        }

        public void Trace(in Visible visible, ref Matrix transformation)
        {
            for (int i = 0; i < visible.Trace.count; i++)
            {
                this.TracePath(in visible.Trace[i], ref transformation);
            }
        }

        private void FillShape(in Shape shape, ref Matrix transformation)
        {
            _fillPrimitiveBatch.EnsureCapacity(shape.Vertices.count);

            ref VertexVisible v1 = ref _fillPrimitiveBatch.NextVertex(out _indexBuffer[0]);
            Vector2.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);

            ref VertexVisible v2 = ref _fillPrimitiveBatch.NextVertex(out _indexBuffer[1]);
            Vector2.Transform(ref shape.Vertices[1], ref transformation, out v2.Position);


            for (int i = 2; i < shape.Vertices.count; i++)
            {
                ref VertexVisible v3 = ref _fillPrimitiveBatch.NextVertex(out _indexBuffer[2]);
                Vector2.Transform(ref shape.Vertices[i], ref transformation, out v3.Position);

                _fillPrimitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
                _fillPrimitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
                _fillPrimitiveBatch.AddTriangleIndex(in _indexBuffer[2]);

                _indexBuffer[1] = _indexBuffer[2];
            }
        }

        private void TracePath(in Shape shape, ref Matrix transformation)
        {
            // _primitiveBatch.EnsureCapacity(shape.Vertices.count);
            // 
            // ref VertexPositionColor v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            // v1.Color = color;
            // Vector2.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);
            // v1.Position = _camera.Project(v1.Position);
            // 
            // for (int i = 1; i < shape.Vertices.count; i++)
            // {
            //     ref VertexPositionColor v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
            //     v2.Color = color;
            //     Vector2.Transform(ref shape.Vertices[i], ref transformation, out v2.Position);
            //     v2.Position = _camera.Project(v2.Position);
            // 
            //     _primitiveBatch.AddLineIndex(in _indexBuffer[0]);
            //     _primitiveBatch.AddLineIndex(in _indexBuffer[1]);
            // 
            //     _indexBuffer[0] = _indexBuffer[1];
            // }
        }
    }
}
