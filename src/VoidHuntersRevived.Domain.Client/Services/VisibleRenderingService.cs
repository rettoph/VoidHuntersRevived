﻿using Guppy.MonoGame;
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
            _indexBuffer = new short[10];
        }

        public void Begin(Color primaryColor, Color secondaryColor)
        {
            _primitiveBatch.BlendState = BlendState.NonPremultiplied;
            _primitiveBatch.Begin(_camera);
            _primitiveBatch.Effect.PrimaryColor = primaryColor;
            _primitiveBatch.Effect.SecondaryColor = secondaryColor;

            float scale = 2 / (-_camera.Zoom - 2) + 1;
            _primitiveBatch.Effect.TraceScale = scale;
            _primitiveBatch.Effect.TraceDiffusionScale = MathHelper.Lerp(scale, 1, 0.75f);
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
                this.Trace(ref visible.TraceVertices[i], ref transformation);
            }
        }

        private void Trace(ref TraceVertices vertices, ref Matrix transformation)
        {
            _primitiveBatch.EnsureCapacity(vertices.Items.count);
            int offset1 = 0;
            int offset2 = 5;
            int placeholder = 0;

            this.GetTraceVertexIndex(ref transformation, ref vertices.Items[offset1 + 0], true,  out _indexBuffer[offset1 + 0]);
            this.GetTraceVertexIndex(ref transformation, ref vertices.Items[offset1 + 1], true,  out _indexBuffer[offset1 + 1]);
            this.GetTraceVertexIndex(ref transformation, ref vertices.Items[offset1 + 2], true,  out _indexBuffer[offset1 + 2]);
            this.GetTraceVertexIndex(ref transformation, ref vertices.Items[offset1 + 3], true,  out _indexBuffer[offset1 + 3]);
            this.GetTraceVertexIndex(ref transformation, ref vertices.Items[offset1 + 4], false, out _indexBuffer[offset1 + 4]);

            for (int i = 5; i < vertices.Items.count; i+=5)
            {
                this.GetTraceVertexIndex(ref transformation, ref vertices.Items[i + 0], true,  out _indexBuffer[offset2 + 0]);
                this.GetTraceVertexIndex(ref transformation, ref vertices.Items[i + 1], true,  out _indexBuffer[offset2 + 1]);
                this.GetTraceVertexIndex(ref transformation, ref vertices.Items[i + 2], true,  out _indexBuffer[offset2 + 2]);
                this.GetTraceVertexIndex(ref transformation, ref vertices.Items[i + 3], true,  out _indexBuffer[offset2 + 3]);
                this.GetTraceVertexIndex(ref transformation, ref vertices.Items[i + 4], false, out _indexBuffer[offset2 + 4]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 1]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 2]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 2]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 3]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 3]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 1]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 1]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 4]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 0]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 4]);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 4]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset2 + 0]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[offset1 + 0]);

                placeholder = offset1;
                offset1 = offset2;
                offset2 = placeholder;
            }

            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);
            //
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[5]);
            //
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[4]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[5]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[6]);
            //
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[5]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[4]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            //
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[3]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[4]);
            //
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[4]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[7]);
            //_primitiveBatch.AddTriangleIndex(in _indexBuffer[3]);

        }

        private void GetTraceVertexIndex(ref Matrix transformation, ref Vector2 trace, bool outer, out short index)
        {
            ref VertexVisible vertex = ref _primitiveBatch.NextVertex(out index);
            Vector2.Transform(ref trace, ref transformation, out vertex.Position);
            vertex.Outer = outer;
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
