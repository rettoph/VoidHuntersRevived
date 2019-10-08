using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Client.Library.Utilities.Cameras;
using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Layers
{
    public class WorldLayer : Layer
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        #endregion

        #region Constructor
        public WorldLayer(GraphicsDevice graphics, SpriteBatch spriteBatch, ClientGalacticFightersWorldScene scene)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _effect = new BasicEffect(_graphics)
            {
                TextureEnabled = true
            };
        }
        #endregion

        #region Frame Methods 
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Update the internal effect
            _effect.Projection = _camera.Projection;
            _effect.View = _camera.View;

            // Draw all entities
            _spriteBatch.Begin(effect: _effect);
            this.entities.TryDraw(gameTime);
            _spriteBatch.End();
        }
        #endregion
    }
}
