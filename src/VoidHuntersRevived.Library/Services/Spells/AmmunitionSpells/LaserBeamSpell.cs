using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.Utilities;
using Guppy.Interfaces;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells
{
    public class LaserBeamSpell : AmmunitionSpell
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private Vector2 _start;
        private Vector2 _end;
        private Int32 _count;
        #endregion

        #region Public Properties
        /// <summary>
        /// The amount of damage done by this laser per second.
        /// </summary>
        public Single DamagePerSecond { get; internal set; }

        /// <summary>
        /// The energy cost to deflect this weapon with shields per second.
        /// </summary>
        public Single EnergyShieldDeflectionCostPerSecond { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);

            this.OnCollision += this.HandleCollision;
            this.Weapon.OnStatus[ServiceStatus.Releasing] += this.HandleLaserReleasing;
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            this.OnCollision -= this.HandleCollision;
            this.Weapon.OnStatus[ServiceStatus.Releasing] -= this.HandleLaserReleasing;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawLine(
                this.Weapon.Chain.Ship?.Color ?? Color.Transparent,
                _start,
                _end);
        }

        protected override void UpdateCollisions(GameTime gameTime)
        {
            _start = this.Weapon.Position;
            _end = this.Weapon.Position + 50f.ToVector2().RotateTo(this.Weapon.Rotation + MathHelper.Pi);
            _count = 0;
            this.TryApplyCollisions(
                start: _start,
                end: _end,
                gameTime: gameTime);
        }
        #endregion


        #region Ammunition Implementation
        /// <inheritdoc />
        public override float GetShieldDeflectionManaCost(CollisionData data, GameTime gameTime)
        {
            return this.EnergyShieldDeflectionCostPerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <inheritdoc />
        public override float GetDamage(CollisionData data, GameTime gameTime)
        {
            return this.DamagePerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion

        #region Event Handlers
        private void HandleCollision(AmmunitionSpell sender, CollisionData collision)
        {
            _count++;
            _end = collision.P1;
        }

        private void HandleLaserReleasing(IService sender)
        {
            this.TryRelease();
        }
        #endregion
    }
}
