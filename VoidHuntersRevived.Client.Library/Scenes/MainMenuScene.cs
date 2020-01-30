using Guppy;
using Guppy.Loaders;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Guppy.UI.Entities.UI;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class MainMenuScene : Scene
    {
        #region Private Fields
        private Camera2D _camera;
        private IServiceProvider _provider;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _backgroundPosition;
        private ClientPeer _client;
        private ContentLoader _content;

        private Texture2D _background01;
        private Texture2D _background02;
        private Texture2D _background03;
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _provider = provider;
            _graphics = provider.GetRequiredService<GraphicsDevice>();
            _client = provider.GetRequiredService<ClientPeer>();
            _spriteBatch = provider.GetRequiredService<SpriteBatch>();
            _content = provider.GetRequiredService<ContentLoader>();

            var content = provider.GetRequiredService<ContentLoader>();
            _background01 = content.TryGet<Texture2D>("sprite:background:1");
            _background02 = content.TryGet<Texture2D>("sprite:background:2");
            _background03 = content.TryGet<Texture2D>("sprite:background:3");

            _spriteBatch = provider.GetRequiredService<SpriteBatch>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _camera = _provider.GetRequiredService<Camera2D>();
            _camera.Center = false;
            _camera.MoveBy(new Vector2(-0.5f, -0.5f));

            this.layers.Create<PrimitiveLayer>(0, l =>
            {
                l.SetCamera(_camera);
            });

            this.entities.Create<Stage>(s =>
            {
                s.Add<Container>(c =>
                {
                    c.Bounds.Height = 100;
                    c.Bounds.Y = 100;
                    c.Bounds.Width = 1f;

                    c.Add<FancyElement>(l =>
                    {
                        l.Bounds.Set(0, 0, 100, 100);
                        l.BackgroundImage = _content.TryGet<Texture2D>("sprite:logo");
                        l.BackgroundStyle = BackgroundStyle.Fill;
                    });

                    c.Add<FancyTextElement>(t =>
                    {
                        t.Bounds.Set(100, 0, Unit.Get(1f, -100), 100);
                        t.Alignment = Alignment.CenterLeft;

                        t.Add<TextElement>(t1 =>
                        {
                            t1.Text = "Void Hunters";
                            t1.Font = _content.TryGet<SpriteFont>("font:ui:title");
                        });

                        t.Add<TextElement>(t2 =>
                        {
                            t2.Text = " Revived";
                            t2.Font = _content.TryGet<SpriteFont>("font:ui:title");
                            t2.Color = new Color(0, 143, 241);
                        });
                    });
                });
            });
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _camera.TryUpdate(gameTime);

            this.layers.TryUpdate(gameTime);

            _backgroundPosition += new Vector2(17f, 7.5f) * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _graphics.Clear(new Color(0, 0, 10));


            var bounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);

            _spriteBatch.Draw(_background01, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_backgroundPosition.X * 1 % _background01.Width, -_backgroundPosition.Y * 1 % _background01.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background02, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_backgroundPosition.X * 2 % _background02.Width, -_backgroundPosition.Y * 2 % _background02.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background03, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_backgroundPosition.X * 3 % _background03.Width, -_backgroundPosition.Y * 3 % _background03.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            _spriteBatch.End();

            _camera.TryDraw(gameTime);

            this.layers.TryDraw(gameTime);

            
        }
        #endregion
    }
}
