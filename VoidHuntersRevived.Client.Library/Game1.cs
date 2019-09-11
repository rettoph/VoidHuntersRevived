using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Guppy.Utilities.Loggers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ClientGalacticFightersGame game;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.game = new GuppyLoader()
                .ConfigureLogger<ConsoleLogger>()
                .ConfigureNetwork("vhr")
                .ConfigureMonoGame(this.graphics, this.Content, this.Window)
                .Initialize()
                .BuildGame<ClientGalacticFightersGame>();
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
    }
}
