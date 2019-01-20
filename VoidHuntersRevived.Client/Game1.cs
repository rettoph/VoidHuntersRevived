using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private VoidHuntersRevivedGame _game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _game = new VoidHuntersRevivedClientGame(new VoidHuntersRevivedLogger(), _graphics, this.Content);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _game.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Update(gameTime);
        }
    }
}
