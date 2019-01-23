using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class ShipPart : TractorableEntity
    {
        private Texture2D _maleConnectionTexture;
        private SpriteBatch _spriteBatch;

        public ShipPart(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(info, game)
        {
            var contentLoader = provider.GetLoader<ContentLoader>();

            _maleConnectionTexture = contentLoader.Get<Texture2D>("texture:male_connection");
            _spriteBatch = spriteBatch;

            this.Visible = true;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.Body.BodyType = BodyType.Dynamic;
            this.Body.Restitution = 0.80f;
            this.Body.Friction = 0f;
            this.Body.LinearDamping = 1f;
            this.Body.AngularDamping = 1f;
            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat2;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Draw(
                texture: _maleConnectionTexture,
                position: this.Body.Position,
                sourceRectangle: _maleConnectionTexture.Bounds,
                color: Color.White,
                rotation: this.Body.Rotation,
                origin: Vector2.Zero,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
