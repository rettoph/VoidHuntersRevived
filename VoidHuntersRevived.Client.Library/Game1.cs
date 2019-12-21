using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Guppy.Utilities.Loggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace VoidHuntersRevived.Client.Library
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ClientGame game;
        String host;

        public Game1(String host = "localhost")
        {
            this.host = host;
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.SynchronizeWithVerticalRetrace = false;

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.IsFixedTimeStep = false;
            this.graphics.PreferMultiSampling = true;
            this.graphics.PreparingDeviceSettings += this.HandlePreparingDeviceSettings;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.game = new GuppyLoader()
                .ConfigureLogger<ConsoleLogger>()
                .ConfigureNetwork("vhr")
                .ConfigureMonoGame(this.graphics, this.Content, this.Window)
                .Initialize()
                .BuildGame<ClientGame>();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.game?.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.game?.TryUpdate(gameTime);
        }

        private void HandlePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}
