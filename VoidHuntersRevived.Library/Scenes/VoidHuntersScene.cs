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
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersScene : Scene
    {
        private ILogger _logger;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;

        public VoidHuntersScene(
            ILogger logger,
            IServiceProvider provider,
            IGame game,
            GraphicsDevice graphics = null,
            SpriteBatch spriteBatch = null
        ) : base(provider, game)
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

            // Create a new game layer
            var layer = this.Layers.Create<GameLayer>();

            // Create a new brick entity
            this.Entities.Create<Brick>("entity:blue_brick", layer);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
