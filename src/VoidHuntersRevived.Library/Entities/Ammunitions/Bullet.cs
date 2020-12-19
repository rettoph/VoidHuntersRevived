using FarseerPhysics.Dynamics;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public class Bullet : Ammunition
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
        /// The maximum allowed age for this bullet in seconds.
        /// Once this is surpassed the bullet will be removed.
        /// </summary>
        public Double MaxAge { get; set; } = 3f;

        /// <summary>
        /// The bullets current age, used to determin when to self delete.
        /// </summary>
        public Double Age { get; private set; }
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

            // Reset the bullet age.
            this.Age = 0;
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

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if((this.Age += gameTime.ElapsedGameTime.TotalSeconds) > this.MaxAge)
            { // Remove the bullet, its too old.
                this.TryRelease();
            }
            else
            { // This bullet has more life yet!
                this.CheckCollisions(
                    this.Position,
                    this.Position += this.Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleCollision(Ammunition sender, ShipPart shipPart)
        {
            // When the bullet collides, we always want to dispose it.
            // Note: Health damage is only applied on the master and is then broadcasted to all slaves.
            // To see that functionality find the BulletMasterNetworkAuthorizationDriver driver.
            this.TryRelease();
        }
        #endregion
    }
}
