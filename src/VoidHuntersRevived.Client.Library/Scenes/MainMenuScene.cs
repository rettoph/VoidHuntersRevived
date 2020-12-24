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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            this.stage.Content.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = new CustomUnit(c => (c - container.Bounds.Width.ToPixel(c)) / 2);
                container.Bounds.Y = 100;
                container.Alignment = StackAlignment.Horizontal;
                container.Children.Create<Element>((logo, p, c) =>
                {
                    logo.Bounds.Height = 75;
                    logo.Bounds.Width = 75;
                    logo.BackgroundImage[ElementState.Default] = p.GetContent<Texture2D>("sprite:ui:logo");
                });

                container.Children.Create<StackContainer>((header, p, c) =>
                {
                    header.Alignment = StackAlignment.Vertical;
                    header.Children.Create<StackContainer>((title, p, c) =>
                    {
                        title.Bounds.Width = 1f;
                        title.Bounds.Height = 100;
                        title.Alignment = StackAlignment.Horizontal;
                        title.Children.Create<TextElement>("ui:label:title", (text, p, c) =>
                        {
                            text.Value = "Void Hunters";
                            text.Font = p.GetContent<SpriteFont>("font:ui:label:bold");
                        });
                        title.Children.Create<TextElement>("ui:label:title", (text, p, c) =>
                        {
                            text.Value = " Revived";
                            text.Font = p.GetContent<SpriteFont>("font:ui:label:light");
                            text.Color[ElementState.Default] = p.GetColor("ui:label:color:2");
                        });
                    });
                    header.Children.Create<TextElement>("ui:label:title:small", (text, p, c) =>
                    {
                        text.Value = "Alpha 0.1.1";
                    });
                });
            });
            #endregion

            this.camera.Zoom = 100f;
            this.camera.Position = new Vector2(Chunk.Size * 3, Chunk.Size * 3) / 2;
            this.camera.MoveLerpStrength = 0.001f;
            this.camera.ZoomLerpStrength = 0.002f;

            _world = this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 3, Chunk.Size * 3);
            });

            _rand = new Random(1);
            for (Int32 i = 0; i < 7; i++)
            {
                this.Entities.Create<ComputerPlayer>((player, p, d) =>
                {
                    player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                    {
                        ship.Import(File.OpenRead("Ships/mosquito.vh"));

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
                sender.Import(File.OpenRead("Ships/mosquito.vh"));

                sender.Bridge.Position = _rand.NextVector2(0, _world.Size.X, 0, _world.Size.Y);
            }
        }
        #endregion
    }
}
