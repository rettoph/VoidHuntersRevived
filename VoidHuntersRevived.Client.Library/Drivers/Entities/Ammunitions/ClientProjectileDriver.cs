using GalacticFighters.Library.Entities.Ammunitions;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.Ammunitions
{
    [IsDriver(typeof(Projectile))]
    public class ClientProjectileDriver : Driver<Projectile>
    {
        #region Private Fields
        private Texture2D _bullet;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Constructor
        public ClientProjectileDriver(SpriteBatch spriteBatch, ContentLoader content, Projectile driven) : base(driven)
        {
            _spriteBatch = spriteBatch;
            _bullet = content.TryGet<Texture2D>("bullet");
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Draw(
                texture: _bullet,
                position: this.driven.Position,
                sourceRectangle: _bullet.Bounds,
                color: Color.White,
                rotation: this.driven.Rotation + MathHelper.PiOver2,
                origin: _bullet.Bounds.Center.ToVector2(),
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
        #endregion
    }
}
