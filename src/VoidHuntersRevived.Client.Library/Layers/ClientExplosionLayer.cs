using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Windows.Library.Graphics.Effects;
using VoidHuntersRevived.Windows.Library.Graphics.Vertices;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Windows.Library.Layers
{
    public class ClientExplosionLayer : ExplosionLayer
    {
        #region Private Fields
        private PrimitiveBatch<VertexExplosion, ExplosionEffect> _primitiveBatch;
        private Camera2D _camera;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);
        }

        protected override void Release()
        {
            base.Release();

            _primitiveBatch = null;
            _camera = null;
        }
        #endregion

        #region Frame Methods
        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _primitiveBatch.Effect.CurrentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;

            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            _primitiveBatch.End();
        }
        #endregion
    }
}
