using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Input.Services;
using Guppy.IO.Services;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network;
using Guppy.Network.Peers;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Client.Library.UI;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Scenes;
using System.Threading;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class MainMenuScene : GraphicsGameScene
    {
        #region Private Fields
        private SceneList _scenes;
        private ServiceList<Player> _players;
        private WorldEntity _world;
        private Random _rand;
        private GraphicsDevice _graphics;
        private Synchronizer _synchronizer;
        private ClientPeer _client;

        private FormComponent _username;
        private FormComponent _host;
        private FormComponent _port;
        private TextElement _connect;
        private User _user;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _scenes);
            provider.Service(out _players);
            provider.Service(out _synchronizer);
            provider.Service(out _client);
            provider.Service(out _user);
            provider.Service(out _graphics);

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);

            _client.OnConnectionStatusChanged += this.HandleClientConnectionStatusChanged;
            _players.OnAdded += this.HandlePlayerAdded;
            _players.OnRemoved += this.HandlePlayerRemoved;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            #region UI
            this.stage.Content.BackgroundColor[ElementState.Default] = new Color(Color.Black, 125);
            this.stage.Content.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = 0;
                container.Bounds.Y = 0.05f;
                container.Alignment = StackAlignment.Vertical;
                container.Inline = InlineType.Vertical;

                container.Children.Create<HeaderComponent>();
                _username = container.Children.Create<FormComponent>((username, p, c) =>
                {
                    username.Label.Value = "Username";
                    username.Input.Value = "Rettoph";
                    username.Bounds.Width = 700;
                    username.Input.Filter = new Regex("^[a-zA-Z0-9]{0,25}$");
                });
                container.Children.Create<StackContainer>((container2, p, c) =>
                {
                    container2.Bounds.X = new CustomUnit(c => (c - container2.Bounds.Width.ToPixel(c)) / 2);
                    container2.Alignment = StackAlignment.Horizontal;
                    _host = container2.Children.Create<FormComponent>((host, p, c) =>
                    {
                        host.Label.Value = "Host";
                        host.Input.Value = "localhost";
                        host.Bounds.Width = 550;
                        host.Input.Filter = new Regex("^[a-zA-Z0-9\\.]{0,100}$");
                    });
                    _port = container2.Children.Create<FormComponent>((port, p, c) =>
                    {
                        port.Label.Value = "Port";
                        port.Input.Value = "1337";
                        port.Bounds.Width = 150;
                        port.Input.Filter = new Regex("^[0-9]{0,5}$");
                    });
                });

                container.Children.Create<Container>((container3, p, c) =>
                {
                    container3.Padding.Top = 25;
                    container3.Inline = InlineType.Vertical;

                    _connect = container3.Children.Create<TextElement>((connect, p, c) =>
                    {
                        connect.Color[ElementState.Default] = Color.White;
                        connect.BackgroundColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                        connect.BackgroundColor[ElementState.Hovered] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.25f);
                        connect.BackgroundColor[ElementState.Pressed] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.5f);
                        connect.Inline = InlineType.None;
                        connect.Bounds.Width = 680;
                        connect.Bounds.Height = 45;
                        connect.Bounds.X = new CustomUnit(c => (c - connect.Bounds.Width.ToPixel(c)) / 2);
                        connect.Alignment = Alignment.CenterCenter;
                        connect.Font = p.GetContent<SpriteFont>("font:ui:normal");
                        connect.Value = "Connect";
                        connect.OnClicked += this.HandleConnectClicked;
                    });
                });
            });
            #endregion

            this.camera.Zoom = 5f;
            var pos = (new Vector2(Chunk.Size * 3, Chunk.Size * 3) / 2);
            pos.Round();
            // pos += new Vector2(0.5f, 0.5f);
            this.camera.Position = pos;
            this.camera.MoveLerpStrength = 0.0001f;
            this.camera.ZoomLerpStrength = 0.0001f;

            _world = this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 3, Chunk.Size * 3);
            });

            _rand = new Random(1);
            for (Int32 i = 0; i < 8; i++)
            {
                this.Entities.Create<ComputerPlayer>((player, p, d) =>
                {
                    player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                    {
                        var ships = Directory.GetFiles("Ships", "*.vh");
                        ship.Import(
                            input: File.OpenRead(ships[_rand.Next(ships.Length)]),
                            position: _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y),
                            rotation: MathHelper.TwoPi * (Single)_rand.NextDouble());
                    });
                });
            }
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

#if DEBUG
            // When debugging, just go right to the game.
            (new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(200);
                _synchronizer.Enqueue(gt => this.HandleConnectClicked(null));
            }))).Start();
#endif
        }

        protected override void Release()
        {
            base.Release();

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Slave);
            this.settings.Set<HostType>(HostType.Remote);

            _players.TryRelease();
            _connect.OnClicked -= this.HandleConnectClicked;
            _players.OnAdded -= this.HandlePlayerAdded;
            _players.OnRemoved -= this.HandlePlayerRemoved;

            _scenes = null;
            _players = null;
            _synchronizer = null;
            _client = null;
            _user = null;
            _graphics = null;

            _username = null;
            _host = null;
            _port = null;

            _username = null;
            _host = null;
            _port = null;
            _connect = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_players.Any())
            {
                this.camera.MoveTo(_players.Where(p => p.Ship.Bridge != default)
                    .Select(p => p.Ship.Bridge.Position)
                    .Aggregate((p1, p2) => p1 + p2) / _players.Count());

                Vector2 min = _world.Size;
                Vector2 max = Vector2.Zero;
                _players.Where(p => p.Ship.Bridge != default).ForEach(p =>
                {
                    min = Vector2.Min(min, p.Ship.Bridge.Position);
                    max = Vector2.Max(max, p.Ship.Bridge.Position);
                });

                Vector2 size = (max - min) * 1.2f;
                Vector2 scale = _graphics.Viewport.Bounds.Size.ToVector2() / size;

                this.camera.ZoomTo(Math.Min(25, Math.Min(scale.X, scale.Y)));
            }
        }
        #endregion

        #region Event Handlers
        private void HandlePlayerBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if(sender.Bridge == default && sender.Status == Guppy.Enums.ServiceStatus.Ready)
            {
                var ships = Directory.GetFiles("Ships", "*.vh");
                sender.Import(
                    input: File.OpenRead(ships[_rand.Next(ships.Length)]),
                    position: _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y),
                    rotation: MathHelper.TwoPi * (Single)_rand.NextDouble());
            }
        }

        private void HandleConnectClicked(Element sender)
        {
            if (_client.ConnectionStatus != NetConnectionStatus.Disconnected)
                return; // Stop here, we can only try connecting if the status is disconnected.

            _user.Name = _username.Input.Value;

            _client.TryConnect(
                host: _host.Input.Value, 
                port: Int32.Parse(_port.Input.Value), 
                user: _user);
        }

        private void HandleClientConnectionStatusChanged(ClientPeer sender, NetConnectionStatus old, NetConnectionStatus value)
        {
            Console.WriteLine(value);

            if(value == NetConnectionStatus.Connected)
            {
                _synchronizer.Enqueue(gt =>
                {
                    var scene = _scenes.Create<GameScene>();
                    _scenes.SetScene(scene);
                    this.TryRelease();
                });
            }
        }

        private void HandlePlayerAdded(IServiceList<Player> sender, Player player)
        {
            player.Ship.OnBridgeChanged += this.HandlePlayerBridgeChanged;
        }

        private void HandlePlayerRemoved(IServiceList<Player> sender, Player player)
        {
            player.Ship.OnBridgeChanged -= this.HandlePlayerBridgeChanged;
        }
        #endregion
    }
}
