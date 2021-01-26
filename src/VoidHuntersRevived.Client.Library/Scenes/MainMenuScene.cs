using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network;
using Guppy.Network.Peers;
using Guppy.UI.Elements;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using VoidHuntersRevived.Client.Library.UI;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using System.Threading;
using Guppy.UI.Enums;
using VoidHuntersRevived.Client.Library.UI.Pages;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class MainMenuScene : GraphicsGameScene
    {
        #region Private Fields
        private SceneList _scenes;
        private ServiceList<Player> _players;
        private WorldEntity _world;
        private GraphicsDevice _graphics;
        private Synchronizer _synchronizer;
        private ClientPeer _client;

        private MainMenuPage _page;
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
            _page = this.stage.Content.Children.Create<MainMenuPage>();
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
            _page.ConnectButton.OnClicked -= this.HandleConnectClicked;
            _players.OnAdded -= this.HandlePlayerAdded;
            _players.OnRemoved -= this.HandlePlayerRemoved;

            _page.TryRelease();

            _scenes = null;
            _players = null;
            _synchronizer = null;
            _client = null;
            _user = null;
            _graphics = null;
            _page = null;
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
                // var ships = Directory.GetFiles("Ships", "*.vh");
                // sender.Import(
                //     input: File.OpenRead(ships[_rand.Next(ships.Length)]),
                //     position: _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y),
                //     rotation: MathHelper.TwoPi * (Single)_rand.NextDouble());
            }
        }

        private void HandleConnectClicked(Element sender)
        {
            if (_client.ConnectionStatus != NetConnectionStatus.Disconnected)
                return; // Stop here, we can only try connecting if the status is disconnected.

            _user.Name = _page.Username;

            _client.TryConnect(
                host: _page.Host, 
                port: _page.Port, 
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
