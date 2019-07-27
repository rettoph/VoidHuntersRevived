using Guppy;
using Guppy.Loggers;
using Guppy.Network;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library;
using Microsoft.Extensions.DependencyInjection;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Guppy.Network.Extensions.Guppy;
using Guppy.Network.Groups;
using Guppy.Network.Drivers;

namespace VoidHuntersRevived.Client.Library
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private GuppyLoader _guppy;
        private VoidHuntersClientGame _game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            _guppy = new GuppyLoader(new ConsoleLogger());

            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferMultiSampling = true;

            this.GraphicsDevice.PresentationParameters.MultiSampleCount = 32;
            this.InactiveSleepTime = TimeSpan.Zero;
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.Title = "Void Hunters Revived 0.0.2";
            // this.Window.IsBorderless = true;

            _graphics.HardwareModeSwitch = false;
            // _graphics.ToggleFullScreen();

            _guppy.ConfigureMonogame(_graphics, this.Window, this.Content);
            _guppy.ConfigureClient();
            _guppy.Initialize();

            _game = _guppy.Games.Create<VoidHuntersClientGame>();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Update(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Environment.Exit(0);
        }
    }
}
