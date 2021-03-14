using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells
{
    public abstract class AmmunitionSpell : Spell
    {
        #region Private Fields
        private WorldEntity _world;
        private List<CollisionData> _discoveredCollisions;
        #endregion

        #region Public Classes
        public class CollisionData
        {
            public AmmunitionSpell Ammunition;
            public IAmmunitionSpellTarget Target;
            public Fixture Fixture;
            public Vector2 P1;
            public Vector2 P2;
            public Single Fraction;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The weapon responsible for firing the current
        /// ammunition.
        /// </summary>
        public Weapon Weapon { get; internal set; }

        /// <summary>
        /// The maximum allowed age for this bullet in seconds.
        /// Once this is surpassed the bullet will be removed.
        /// </summary>
        public Double MaxAge { get; internal set; } = 3f;

        /// <summary>
        /// The bullets current age, used to determin when to self delete.
        /// </summary>
        public Double Age { get; private set; }
        #endregion

        #region Events
        public OnEventDelegate<AmmunitionSpell, CollisionData> OnCollision;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _discoveredCollisions = new List<CollisionData>();
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
            base.Release();

            _world = null;
            _discoveredCollisions = null;

            this.Weapon = null;

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
        public void TryApplyCollisions(Vector2 start, Vector2 end, GameTime gameTime)
        {
            _discoveredCollisions.Clear();

            _world.Live.RayCast(
                this.HandleCollision,
                start,
                end);

            if (_discoveredCollisions.Any())
            {
                foreach (CollisionData collision in this.GetValidCollisions(_discoveredCollisions))
                {
                    collision.Target.ApplyAmmunitionCollision(collision, gameTime);
                    this.OnCollision?.Invoke(this, collision);
                }
            }
        }

        /// <summary>
        /// Return a collection of collisions that should
        /// actually be applied. Default dunctionality
        /// will return the first collision only.
        /// </summary>
        /// <param name="collisions"></param>
        /// <returns></returns>
        protected virtual IEnumerable<CollisionData> GetValidCollisions(IEnumerable<CollisionData> collisions)
        {
            yield return collisions.MinBy(collisions => collisions.Fraction);
        }

        /// <summary>
        /// Calculate the cost of a blocking this projectile with an energy
        /// shield. Since this can be a static number (like with projectiles)
        /// or fluctuate based on time (like with lasers).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public abstract Single GetShieldDeflectionManaCost(CollisionData data, GameTime gameTime);

        /// <summary>
        /// Calculate the amount of damage to applied to a ship part based on the
        /// frame's elapsed gametime.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public abstract Single GetDamage(CollisionData data, GameTime gameTime);
        #endregion

        #region Event Handlers
        private float HandleCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float fraction)
        {
            if (arg1.Tag is IAmmunitionSpellTarget target)
            {
                var collisionData = new CollisionData()
                {
                    Ammunition = this,
                    Target = target,
                    Fixture = arg1,
                    P1 = arg2,
                    P2 = arg3,
                    Fraction = fraction,
                };

                if (collisionData.Target.ValidateAmmunitionCollision(collisionData))
                    _discoveredCollisions.Add(collisionData);
            }

            return 1f;
        }
        #endregion
    }
}
