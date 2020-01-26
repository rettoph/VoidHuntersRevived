using Guppy;
using Guppy.Loaders;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Guppy.UI.Entities.UI;
using Guppy.UI.Utilities.Units;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities.UI;
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

        private Texture2D _background01;
        private Texture2D _background02;
        private Texture2D _background03;

        private FormComponent _name;
        private FormComponent _host;
        private FormComponent _port;
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _provider = provider;
            _graphics = provider.GetRequiredService<GraphicsDevice>();
            _client = provider.GetRequiredService<ClientPeer>();
            _spriteBatch = provider.GetRequiredService<SpriteBatch>();

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
            _camera.MoveBy(new Vector2(0.5f, 0.5f));

            this.layers.Create<PrimitiveLayer>(0, l =>
            {
                l.SetCamera(_camera);
            });

            this.entities.Create<Stage>(s =>
            {
                s.Add<Container>(c =>
                {
                    c.Bounds.Set(new Unit[] { 0.5f, -250 }, 100, 500, 290);
                    c.BorderSize = 0;
                    c.BackgroundColor = new Color(23, 140, 182, 200);

                    c.Add<FormComponent>(fc =>
                    {
                        fc.Label = "Name:";
                        fc.Bounds.Top = 0;
                    });

                    c.Add<FormComponent>(fc =>
                    {
                        fc.Label = "Host:";
                        fc.Bounds.Top = 70;
                    });

                    c.Add<FormComponent>(fc =>
                    {
                        fc.Label = "Port:";
                        fc.Bounds.Top = 140;
                    });

                    c.Add<TextButton>(b =>
                    {
                        b.Bounds.Set(15, 230, new Unit[] { 1f, -30  }, 45);
                        b.BackgroundColor = new Color(13, 31, 45);
                        b.BorderSize = 0;
                        b.Text = "Connect";
                        b.OnHoveredChanged += (o, h) =>
                        {
                            b.BackgroundColor = h ? new Color(29, 70, 102) : b.BackgroundColor = new Color(13, 31, 45);
                        };
                        b.OnButtonPressed += (o, a) =>
                        {
                            if(a == Pointer.Button.Left)
                                b.BackgroundColor = new Color(10, 24, 35);
                        };
                        b.OnButtonReleased += (o, a) =>
                        {
                            if(a == Pointer.Button.Left)
                                b.BackgroundColor = new Color(29, 70, 102);
                        };
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
