using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.Ammunitions;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Ammunitions
{
    /// <summary>
    /// Simple driver dedicated to rendering a projectile's
    /// texture.
    /// </summary>
    [IsDriver(typeof(Projectile))]
    internal sealed class ProjectileTextureDriver : Driver<Projectile>
    {
        #region Private Fields
        private ContentLoader _content;
        private Texture2D _texture;
        private Vector2 _origin;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Constructor
        public ProjectileTextureDriver(SpriteBatch spriteBatch, ContentLoader content, Projectile driven) : base(driven)
        {
            _content = content;
            _spriteBatch = spriteBatch;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Load the projectiles texture, if any.
            _texture = _content.TryGet<Texture2D>($"texture:{this.driven.Handle}");
            _origin = _texture.Bounds.Center.ToVector2();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw the projectile...
            _spriteBatch.Draw(
                texture: _texture,
                position: this.driven.Position,
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: this.driven.Rotation,
                origin: _origin,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
        #endregion
    }
}
