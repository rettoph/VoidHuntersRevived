using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Library.Layers
{
    public class GameLayer : Layer
    {
        private SpriteBatch _spriteBatch;

        public GameLayer(SpriteBatch spriteBatch, IGame game) : base(game)
        {
            this.Visible = true;
            this.Enabled = true;

            _spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            this.Entities.Draw(gameTime);

            _spriteBatch.End();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }
    }
}
