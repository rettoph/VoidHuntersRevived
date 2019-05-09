using Guppy;
using Guppy.Configurations;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class HudLayer : Layer
    {
        private BasicEffect _effect;
        private GameWindow _window;
        private Camera2D _camera;

        protected SpriteBatch spriteBatch;

        public HudLayer(LayerConfiguration configuration, IServiceProvider provider, GameWindow window, BasicEffect effect, SpriteBatch spriteBatch, Camera2D camera) : base(configuration, provider, camera)
        {
            _camera = camera;
            _window = window;
            _effect = effect;
            _effect.TextureEnabled = true;
            _camera.MoveTo(_window.ClientBounds.Width / 2, _window.ClientBounds.Height / 2);

            this.spriteBatch = spriteBatch;

            _window.ClientSizeChanged += this.HandleClientBoundsChanged;
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(effect: _effect);
            this.entities.Draw(gameTime);
            this.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            this.Camera.Update(gameTime);
            this.entities.Update(gameTime);

            _effect.Projection = this.Camera.Projection;
            _effect.World = this.Camera.World;
        }

        private void HandleClientBoundsChanged(object sender, EventArgs e)
        {
            _camera.MoveTo(_window.ClientBounds.Width / 2, _window.ClientBounds.Height / 2);
        }
    }
}
