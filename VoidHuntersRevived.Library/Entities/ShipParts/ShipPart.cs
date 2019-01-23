﻿using System;
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
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class ShipPart : TractorableEntity
    {
        public readonly ShipPartData ShipPartData;

        private Texture2D _maleConnectionTexture;
        private SpriteBatch _spriteBatch;
        protected Matrix _rotationMatrix;

        public ShipPart(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(info, game)
        {
            var contentLoader = provider.GetLoader<ContentLoader>();
            _maleConnectionTexture = contentLoader.Get<Texture2D>("texture:male_connection");

            _spriteBatch = spriteBatch;

            this.Visible = true;

            this.ShipPartData = info.Data as ShipPartData;
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

            var maleJointOffset = Vector2.Transform(this.ShipPartData.MaleConnection.LocalPoint, _rotationMatrix);
            _spriteBatch.Draw(
                texture: _maleConnectionTexture,
                position: this.Body.Position + maleJointOffset,
                sourceRectangle: _maleConnectionTexture.Bounds,
                color: Color.White,
                rotation: this.Body.Rotation,
                origin: Vector2.Zero,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _rotationMatrix = Matrix.CreateRotationZ(this.Body.Rotation);
        }
    }
}
