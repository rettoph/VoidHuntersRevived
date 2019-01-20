﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.MetaData;

namespace VoidHuntersRevived.Library.Entities
{
    public class Brick : Entity
    {
        private Texture2D _texture;
        private SpriteBatch _spriteBatch;

        public Brick(IServiceProvider provider, SpriteBatch spriteBatch, EntityInfo info, IGame game) : base(info, game)
        {
            _texture = provider.GetLoader<ContentLoader>().Get<Texture2D>("basic_brick");
            _spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(_texture, Vector2.Zero, (this.Info.Data as BrickData).Color);
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
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
