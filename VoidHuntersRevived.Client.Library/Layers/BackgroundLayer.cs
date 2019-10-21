using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Loaders;
using Guppy.Pooling.Interfaces;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Effects;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class BackgroundLayer : BloomLayer
    {
        private SpriteBatch _spriteBatch;
        private ContentLoader _content;
        private Camera2D _camera;
        private GraphicsDevice _graphics;

        private Texture2D _background01;
        private Texture2D _background02;
        private Texture2D _background03;


        #region Constructor
        public BackgroundLayer(ContentLoader content, SpriteBatch spriteBatch, GraphicsDevice graphics, GameWindow window, ClientWorldScene scene) : base(content, spriteBatch, graphics, window)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _camera = scene.Camera;
            _graphics = graphics;
        }
        #endregion

        #region Life Cycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _background01 = _content.TryGet<Texture2D>("sprite:background:1");
            _background02 = _content.TryGet<Texture2D>("sprite:background:2");
            _background03 = _content.TryGet<Texture2D>("sprite:background:3");

            _spriteBatch = new SpriteBatch(_graphics);

            this.SetDrawOrder(0);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            this.bloom.BloomThreshold = 0.1f;
            this.bloom.BloomStrengthMultiplier = 0.9f;
            this.bloom.BloomUseLuminance = false;
            this.bloom.BloomPreset = Effects.BloomFilter.BloomPresets.SuperWide;

            base.Draw(gameTime);

            var bounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);

            _spriteBatch.Draw(_background01, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 1 % _background01.Width, -_camera.Position.Y * 1 % _background01.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background02, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 2 % _background02.Width, -_camera.Position.Y * 2 % _background02.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background03, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 3 % _background03.Width, -_camera.Position.Y * 3 % _background03.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            _spriteBatch.End();
        }
        #endregion
    }
}
