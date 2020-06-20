using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Drivers.Layers
{
    internal sealed class GameLayerGraphicsDriver : Driver<GameLayer>
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private FarseerCamera2D _camera;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _spriteBatch);
            provider.Service(out _camera);

            this.driven.OnPreDraw += this.PreDraw;
            this.driven.OnPostDraw += this.PostDraw;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.OnPreDraw -= this.PreDraw;
            this.driven.OnPostDraw -= this.PostDraw;
        }
        #endregion

        #region Frame Methods
        private void PreDraw(GameTime gameTime)
            => _primitiveBatch.Begin(_camera.View, _camera.Projection);

        private void PostDraw(GameTime gameTime)
            => _primitiveBatch.End();
        #endregion
    }
}
