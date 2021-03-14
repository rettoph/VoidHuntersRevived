using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells
{
    public class BulletSpell : AmmunitionSpell
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        #endregion

        #region Public Properties
        /// <summary>
        /// The amount of damage to be applied to
        /// a target <see cref="ShipPart"/> on
        /// <see cref="ShipPart.Hit(Ammunition)"/>
        /// </summary>
        public Single Damage { get; set; }

        /// <summary>
        /// The bullets current world position.
        /// </summary>
        public Vector2 Position { get; internal set; }

        /// <summary>
        /// The bullets current velocity (per second).
        /// </summary>
        public Vector2 Velocity { get; internal set; }

        /// <summary>
        /// The bullet's shield deflection energy cost.
        /// </summary>
        public Single EnergyShieldDeflectionCost { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnCollision += this.HandleCollision;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
        }

        protected override void Release()
        {
            base.Release();

            _primitiveBatch = null;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnCollision -= this.HandleCollision;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawLine(Color.Red, this.Position, this.Position + (this.Velocity * 0.05f));
        }

        protected override void UpdateCollisions(GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.TotalSeconds == 0)
                return;

            this.TryApplyCollisions(
                start: this.Position,
                end: this.Position += this.Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds,
                gameTime: gameTime);
        }
        #endregion

        #region Ammunition Implementation
        /// <inheritdoc />
        public override float GetShieldDeflectionManaCost(CollisionData data, GameTime gameTime)
        {
            return this.EnergyShieldDeflectionCost;
        }

        /// <inheritdoc />
        public override float GetDamage(CollisionData data, GameTime gameTime)
        {
            return this.Damage;
        }
        #endregion

        #region Event Handlers
        private void HandleCollision(AmmunitionSpell sender, CollisionData data)
        {
            // When the bullet collides, we always want to dispose it.
            // Note: Health damage is only applied on the master and is then broadcasted to all slaves.
            // To see that functionality find the ShipPartMasterNetworkAuthorizationDriver driver.
            this.TryRelease();
        }
        #endregion
    }
}
