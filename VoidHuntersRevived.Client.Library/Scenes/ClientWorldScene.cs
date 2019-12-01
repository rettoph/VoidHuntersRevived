using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientWorldScene : WorldScene
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        #endregion

        #region Constructor
        public ClientWorldScene(BasicEffect effect, FarseerCamera2D camera, SpriteBatch spriteBatch)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
            _effect = effect;

            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _effect.Projection = _camera.Projection;
            _effect.View = _camera.View;

            _spriteBatch.Begin(effect: _effect);
            this.entities.TryDraw(gameTime);
            _spriteBatch.End();
        }
        #endregion
    }
}
