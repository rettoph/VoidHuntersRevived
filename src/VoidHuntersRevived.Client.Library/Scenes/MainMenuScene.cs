using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class MainMenuScene : GraphicsGameScene
    {
        #region Private Fields
        private ServiceList<Player> _players;
        private WorldEntity _world;
        private Random _rand;
        private GraphicsDevice _graphics;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _players);

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _graphics);

            #region UI
            this.stage.Content.BackgroundColor[ElementState.Default] = new Color(Color.Black, 125);
            this.stage.Content.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = 0;
                container.Bounds.Y = 0.05f;
                container.Alignment = StackAlignment.Vertical;
                container.Inline = InlineType.Vertical;

                container.Children.Create<HeaderComponent>();
                container.Children.Create<FormComponent>((username, p, c) =>
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
                    container2.Children.Create<FormComponent>((host, p, c) =>
                    {
                        host.Label.Value = "Host";
                        host.Input.Value = "localhost";
                        host.Bounds.Width = 550;
                        host.Input.Filter = new Regex("^[a-zA-Z0-9]{0,100}$");
                    });
                    container2.Children.Create<FormComponent>((port, p, c) =>
                    {
                        port.Label.Value = "Port";
                        port.Input.Value = "1337";
                        port.Bounds.Width = 150;
                        port.Input.Filter = new Regex("^[0-9]{0,5}$");
                    });
                });
            });
            #endregion

            this.camera.Zoom = 10f;
            this.camera.Position = new Vector2(Chunk.Size * 3, Chunk.Size * 3) / 2;
            this.camera.MoveLerpStrength = 0.0001f;
            this.camera.ZoomLerpStrength = 0.0001f;

            _world = this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 3, Chunk.Size * 3);
            });

            _rand = new Random(1);
            for (Int32 i = 0; i < 25; i++)
            {
                this.Entities.Create<ComputerPlayer>((player, p, d) =>
                {
                    player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                    {
                        var ships = Directory.GetFiles("Ships", "*.vh");
                        ship.Import(File.OpenRead(ships[_rand.Next(ships.Length)]));

                        // ship.SetBridge(this.Entities.Create<ShipPart>("entity:ship-part:chassis:mosquito"));
                        ship.Bridge.Position = _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y);
                    });
                    player.Ship.OnBridgeChanged += this.HandlePlayerBridgeChanged;
                });
            }
        }

        protected override void Release()
        {
            base.Release();

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Slave);
            this.settings.Set<HostType>(HostType.Remote);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            this.camera.ZoomTo(Math.Min(scale.X, scale.Y));
        }
        #endregion

        #region Event Handlers
        private void HandlePlayerBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if(sender.Bridge == default)
            {
                var ships = Directory.GetFiles("Ships", "*.vh");
                sender.Import(File.OpenRead(ships[_rand.Next(ships.Length)]));

                sender.Bridge.Position = _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y);
            }
        }
        #endregion
    }
}
