using Guppy;
using Guppy.Collections;
using Guppy.Loaders;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Guppy.UI.Entities;
using Guppy.UI.Entities.UI;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private ContentLoader _content;

        private Texture2D _background01;
        private Texture2D _background02;
        private Texture2D _background03;

        private TextElement _message;

        private NetConnectionStatus _status = NetConnectionStatus.Disconnected;

        private SceneCollection _scenes;
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

            _scenes = provider.GetRequiredService<SceneCollection>();
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
                s.Add<Header>(h =>
                {
                    h.Bounds.Set(0, 0.1f, 1f, 75);
                });

                _message = s.Add<TextElement>(m =>
                {
                    m.Bounds.Y = Unit.Get(0.1f, 85);
                    m.Bounds.Height = 25;
                    m.Inline = false;
                    m.Alignment = Alignment.Center;
                    m.Font = _content.TryGet<SpriteFont>("font:ui:input");
                });

                s.Add<Container>(c =>
                {
                    // c.BorderColor = Color.White;
                    // c.BorderSize = 1;

                    c.Bounds.Set(
                        x: new CustomUnit(p => (p - c.Bounds.Width.ToPixel(p)) / 2),
                        y: Unit.Get(0.1f, 100),
                        width: 593,
                        height: 300);

                    var name = c.Add<FormComponent>(fc =>
                    {
                        fc.Bounds.Set(25, 25, 543, 60);
                        fc.Label = "Name";
                    });

                    var host = c.Add<FormComponent>(fc =>
                    {
                        fc.Bounds.Set(25, 110, 393, 60);
                        fc.Label = "Host";
                        fc.Value = "localhost";
                    });

                    var port = c.Add<FormComponent>(fc =>
                    {
                        fc.Bounds.Set(443, 110, 125, 60);
                        fc.Label = "Port";
                        fc.Value = "1337";
                    });

                    c.Add<Container>(c2 =>
                    {
                        c2.Bounds.Set(25, 210, 543, 45);
                        c2.BorderColor = new Color(0, 143, 241);
                        c2.BackgroundColor = new Color(50, 140, 200, 100);
                        c2.BorderSize = 1;

                        c2.Add<TextElement>(t =>
                        {
                            t.Alignment = Alignment.Center;
                            t.Inline = false;
                            t.Text = "Connect";
                            t.Font = _content.TryGet<SpriteFont>("font:ui:label");
                        });

                        c2.OnHoveredChanged += (sender, h) =>
                        {
                            if ((c2.Buttons & Pointer.Button.Left) == 0)
                                c2.BackgroundColor = h ? new Color(44, 123, 175, 100) : new Color(50, 140, 200, 100);
                        };

                        c2.OnButtonPressed += (sender, b) =>
                        {
                            if (b == Pointer.Button.Left)
                                c2.BackgroundColor = new Color(71, 154, 209, 100);
                        };

                        c2.OnButtonReleased += (sender, b) =>
                        {
                            if (b == Pointer.Button.Left)
                            {
                                c2.BackgroundColor = c2.Hovered ? new Color(44, 123, 175, 100) : new Color(50, 140, 200, 100);
                                if (c2.Hovered)
                                    try
                                    {
                                        this.Connect(host.Value, Int32.Parse(port.Value), name.Value);
                                    }
                                    catch (Exception e)
                                    {
                                        _message.Text = $"Error: {e.Message}";
                                        _message.Color = Color.Red;
                                    }
                            }

                        };
                    });
                });
            });
        }

        protected override void Initialize()
        {
            base.Initialize();

            _client.MessagesTypes.TryAdd(NetIncomingMessageType.StatusChanged, this.HandleConnectionStatusChanged);

            #if DEBUG
            this.Connect("localhost", 1337, "Rettoph");
            #endif
        }

        public override void Dispose()
        {
            base.Dispose();

            _client.MessagesTypes.TryRemove(NetIncomingMessageType.StatusChanged, this.HandleConnectionStatusChanged);
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

        #region Methods
        /// <summary>
        /// Attempt to connect to the requested server
        /// </summary>
        private void Connect(String host, Int32 port, String name)
        {
            if(_client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                var t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        _client.TryConnect(host, port, _client.Users.Create(name));
                    }
                    catch (Exception e)
                    {
                        _message.Text = $"Error: {e.Message}";
                        _message.Color = Color.Red;
                    }
                }));
                t.Start();
            }
        }
        #endregion

        #region Message Handlers
        private void HandleConnectionStatusChanged(object sender, NetIncomingMessage arg)
        {
            _status = (NetConnectionStatus)arg.ReadByte();

            switch(_status) {
                case NetConnectionStatus.InitiatedConnect:
                    _message.Text = "Connecting to server...";
                    _message.Color = Color.Yellow;
                    break;
                case NetConnectionStatus.Connected:
                    _message.Text = $"Connected!";
                    _message.Color = Color.Green;

                    _scenes.Create<ClientWorldScene>(s =>
                    {
                        s.Group = _client.Groups.GetOrCreateById(Guid.Empty);
                    });
                    this.Dispose();
                    break;
                default:
                    _message.Text = $"Error: Unable to reach host... Please try again.";
                    _message.Color = Color.Red;
                    break;
            }
        }
        #endregion
    }
}
