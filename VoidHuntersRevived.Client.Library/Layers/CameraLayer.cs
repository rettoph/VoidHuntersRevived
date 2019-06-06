using Guppy;
using Guppy.Configurations;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class CameraLayer : Layer
    {
        private BasicEffect _effect;

        protected SpriteBatch spriteBatch;
        protected FarseerCamera2D camera;

        public CameraLayer(FarseerCamera2D camera, LayerConfiguration configuration, BasicEffect effect, SpriteBatch spriteBatch, IServiceProvider provider, ILogger logger) : base(configuration, provider, logger, camera)
        {
            _effect = effect;
            _effect.TextureEnabled = true;

            this.spriteBatch = spriteBatch;
            this.camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            _effect.Projection = this.camera.Projection;
            _effect.World = this.camera.World;

            this.spriteBatch.Begin(effect: _effect);
            this.entities.Draw(gameTime);
            this.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            this.entities.Update(gameTime);
        }
    }
}
