using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public abstract class Ammunition : Entity
    {
        #region Private Fields
        private WorldEntity _world;
        private CollisionData _closestCollision;
        private Single _smallestFraction;
        #endregion

        #region Public Classes
        public class CollisionData
        {
            public Ammunition Ammunition;
            public ShipPart Target;
            public Fixture Fixture;
            public Vector2 P1;
            public Vector2 P2;
            public GameTime GameTime;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The id of the chain that is responsible
        /// for firing this ammunition. Used to ignore
        /// internal collisions.
        /// </summary>
        public Guid ShooterId { get; set; }

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

        #region Events
        public OnEventDelegate<Ammunition, CollisionData> OnCollision;
        public ValidateEventDelegate<Ammunition, CollisionData> ValidateCollision;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.ValidateCollision += this.HandleValidateCollision;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _world);

            // Reset the bullet age.
            this.Age = 0;
        }

        protected override void Release()
        {
            _world = null;

            base.Release();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.ValidateCollision -= this.HandleValidateCollision;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if ((this.Age += gameTime.ElapsedGameTime.TotalSeconds) > this.MaxAge)
            { // Remove the bullet, its too old.
                this.TryRelease();
            }
            else
            { // This bullet has more life yet!
                this.UpdateCollisions(gameTime);
            }
        }

        protected abstract void UpdateCollisions(GameTime gameTime);
        #endregion

        #region Helper Methods

        /// <summary>
        /// Check all collisions between the given start and end position,
        /// if any. Handle any found collisions as needed.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="gameTime"></param>
        public void CheckCollisions(Vector2 start, Vector2 end, GameTime gameTime)
        {
            _closestCollision = default;
            _smallestFraction = 1f;

            _world.Live.RayCast(
                this.HandleCollision,
                start,
                end);

            if (_closestCollision != default)
            {
                _closestCollision.GameTime = gameTime;
                this.OnCollision?.Invoke(this, _closestCollision);
            }
        }
        #endregion

        #region Event Handlers
        private float HandleCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float fraction)
        {
            if (arg1.Tag is ShipPart target && fraction < _smallestFraction)
            {
                var collisionData = new CollisionData()
                {
                    Ammunition = this,
                    Target = target,
                    Fixture = arg1,
                    P1 = arg2,
                    P2 = arg3
                };

                if (this.ValidateCollision.Validate(this, collisionData, true))
                {
                    _smallestFraction = fraction;
                    _closestCollision = collisionData;

                }
            }

            return 1f;
        }

        /// <summary>
        /// Default implementation to validate that a bullet collision
        /// is acceptable. By default, collisions must
        /// occur to parts that belong to a ship and have
        /// over 0 health.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shipPart"></param>
        /// <returns></returns>
        private bool HandleValidateCollision(Ammunition sender, CollisionData data)
            => data.Target.Chain.Id != this.ShooterId && data.Target.Health > 0 && data.Target.Chain.Ship != default;
        #endregion
    }
}
