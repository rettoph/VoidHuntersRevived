using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Windows.Library.Layers
{
    public class ClientSpellLayer : SpellLayer
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _vertexPositionColorPrimitiveBatch;
        private Camera2D _camera;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _vertexPositionColorPrimitiveBatch);
            provider.Service(out _camera);

            _vertexPositionColorPrimitiveBatch.Effect.VertexColorEnabled = true;
        }

        protected override void Release()
        {
            base.Release();

            _vertexPositionColorPrimitiveBatch = null;
            _camera = null;
        }
        #endregion

        #region Frame Methods
        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _vertexPositionColorPrimitiveBatch.Begin(_camera);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            _vertexPositionColorPrimitiveBatch.End();
        }
        #endregion
    }
}
