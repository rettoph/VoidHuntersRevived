using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private readonly GraphicsDeviceManager _graphics;
        private VoidHuntersRevivedGame _game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            this.IsMouseVisible = false;
            this.Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleCount = 32;
            _graphics.ApplyChanges();

            var collection = new ServiceCollection();
            collection.AddSingleton<Game>(this);

            _game = new ClientVoidHuntersRevivedGame(new VoidHuntersRevivedLogger(), _graphics, this.Content, this.Window, collection);
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
