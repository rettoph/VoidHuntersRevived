using Guppy;
using Guppy.Utilities.Cameras;
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

        #endregion

        #region Protected Fields
        protected SpriteBatch spriteBatch;
        protected BasicEffect effect;
        protected Camera2D camera;
        #endregion

        #region Constructor
        public CameraLayer(SpriteBatch spriteBatch, BasicEffect effect, FarseerCamera2D camera)
        {
            this.spriteBatch = spriteBatch;
            this.effect = effect;
            this.camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.effect.TextureEnabled = true;
            this.effect.VertexColorEnabled = true;
        }
        #endregion

        #region Frame Methods
        protected override void PreDraw(GameTime gameTIme)
        {
            base.PreDraw(gameTIme);

            this.effect.View = this.camera.View;
            this.effect.Projection = this.camera.Projection;

            this.spriteBatch.Begin(sortMode: SpriteSortMode.Texture, effect: this.effect);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Entities.TryDraw(gameTime);
        }

        protected override void PostDraw(GameTime gameTIme)
        {
            base.PostDraw(gameTIme);

            this.spriteBatch.End();
        }
        #endregion

        #region Setters
        public void SetCamera(Camera2D camera)
        {
            this.camera = camera;
        }
        #endregion
    }
}
