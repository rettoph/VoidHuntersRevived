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
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.game = new GuppyLoader()
                .ConfigureLogger<ConsoleLogger>()
                .ConfigureNetwork("vhr")
                .ConfigureMonoGame(this.graphics, this.Content, this.Window)
                .Initialize()
                .BuildGame<ClientGalacticFightersGame>(g =>
                {
                    g.host = this.host;
                });
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
