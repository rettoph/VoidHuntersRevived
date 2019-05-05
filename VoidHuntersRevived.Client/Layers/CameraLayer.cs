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
        protected SpriteBatch spriteBatch;
        protected Camera camera;

        public CameraLayer(Camera2D camera, Scene scene, LayerConfiguration configuration, SpriteBatch spriteBatch, GameWindow window, GraphicsDevice graphicsDevice) : base(scene, configuration, window, graphicsDevice)
        {
            this.spriteBatch = spriteBatch;
            this.camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin();
            this.entities.Draw(gameTime);
            this.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            this.camera.Update(gameTime);
            this.entities.Update(gameTime);
        }
    }
}
