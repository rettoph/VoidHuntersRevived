using Guppy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Layers
{
    /// <summary>
    /// Simple layer that will automatically
    /// draw & update all contained entities.
    /// </summary>
    public class CameraLayer : BasicLayer
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private BasicEffect _effect;
        private FarseerCamera2D _camera;
        #endregion

        #region Constructor
        public CameraLayer(SpriteBatch spriteBatch, BasicEffect effect, FarseerCamera2D camera)
        {
            _spriteBatch = spriteBatch;
            _effect = effect;
            _camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;

            _spriteBatch.Begin(effect: _effect);
            this.entities.TryDraw(gameTime);
            _spriteBatch.End();
        }
        #endregion
    }
}
