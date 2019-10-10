using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Client.Library.Utilities.Cameras;
using GalacticFighters.Library.Entities.Ammunitions;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
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
        private Vector2 _origin;
        private FarseerCamera2D _camera;
        private Vector3 _position;
        #endregion

        #region Constructor
        public ClientProjectileDriver(ClientGalacticFightersWorldScene scene, SpriteBatch spriteBatch, ContentLoader content, Projectile driven) : base(driven)
        {
            _camera = scene.Camera;
            _spriteBatch = spriteBatch;
            _bullet = content.TryGet<Texture2D>("bullet");
            _origin = _bullet.Bounds.Center.ToVector2();
            _position = new Vector3(0, 0, 0);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Update the internal Vector3 data
            this.driven.Position.Deconstruct(out _position.X, out _position.Y);

            if (_camera.Frustum.Contains(_position).HasFlag(ContainmentType.Contains))
            {
                _spriteBatch.Draw(
                    texture: _bullet,
                    position: this.driven.Position,
                    sourceRectangle: _bullet.Bounds,
                    color: Color.White,
                    rotation: this.driven.Rotation,
                    origin: _origin,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
        #endregion
    }
}
