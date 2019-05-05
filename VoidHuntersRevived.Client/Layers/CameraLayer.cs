using Guppy;
using Guppy.Configurations;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Layers
{
    public class CameraLayer : Layer
    {
        private BasicEffect _effect;

        protected SpriteBatch spriteBatch;
        protected Camera camera;

        public CameraLayer(Camera2D camera, LayerConfiguration configuration, BasicEffect effect, SpriteBatch spriteBatch, IServiceProvider provider) : base(configuration, provider, camera)
        {
            _effect = effect;
            _effect.TextureEnabled = true;

            this.spriteBatch = spriteBatch;
            this.camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(effect: _effect);
            this.entities.Draw(gameTime);
            this.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            this.camera.Update(gameTime);
            this.entities.Update(gameTime);

            _effect.Projection = this.camera.Projection;
            _effect.World = this.camera.World;
        }
    }
}
