using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersScene : Scene
    {
        private ILogger _logger;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;

        public VoidHuntersScene(
            ILogger logger,
            IGame game,
            GraphicsDevice graphics = null,
            SpriteBatch spriteBatch = null
        ) : base(game)
        {
            _logger = logger;
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            this.Enabled = true;
            this.Visible = true;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            // Create a new brick entity
            this.Entities.Create<Brick>();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _graphics.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            this.Entities.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
