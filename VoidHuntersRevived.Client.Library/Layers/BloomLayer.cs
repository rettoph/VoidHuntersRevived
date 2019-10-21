using Guppy;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Effects;

namespace VoidHuntersRevived.Client.Library.Layers
{
    /// <summary>
    /// Simple layer that will automatically apply
    /// a bloom filter to all drawn objects.
    /// </summary>
    public class BloomLayer : Layer
    {
        #region Private Fields
        private ContentLoader _content;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private GameWindow _window;
        private RenderTarget2D _renderTarget;
        private RenderTarget2D _output;
        #endregion

        #region Protected Attributes
        protected BloomFilter bloom { get; private set; }
        #endregion

        #region Constructor
        public BloomLayer(ContentLoader content, SpriteBatch spriteBatch, GraphicsDevice graphics, GameWindow window)
        {
            _content = content;
            _spriteBatch = spriteBatch;
            _graphics = graphics;
            _window = window;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.bloom = new BloomFilter();
            this.Resize();

            _window.ClientSizeChanged += this.HandleClientSizedChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            _window.ClientSizeChanged -= this.HandleClientSizedChanged;

            this.bloom.Dispose();
        }
        #endregion

        #region Frame Methods
        public override void TryDraw(GameTime gameTime)
        {
            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(Color.Transparent);
            
            base.TryDraw(gameTime);
            
            // Proccess the bloom output
            var processed = this.bloom.Draw(_renderTarget, _renderTarget.Width, _renderTarget.Height);;

            // Draw the bloom
            _graphics.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            _spriteBatch.Draw(processed, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
        #endregion

        #region Helper Methods
        private void Resize()
        {
            this.bloom.Load(_graphics, _content, _graphics.Viewport.Width, _graphics.Viewport.Height);
            _renderTarget = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height);
            _output = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height);
        }
        #endregion

        #region Event Handlers
        private void HandleClientSizedChanged(object sender, EventArgs e)
        {
            this.Resize();
        }
        #endregion
    }
}
