using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public abstract class Ammunition : Entity
    {
        #region Private Fields
        private WorldEntity _world;
        private List<CollisionDataResult> _validCollisions;
        private GameTime _currentGameTime;
        #endregion

        #region Public Classes
        public class CollisionData
        {
            public Ammunition Ammunition;
            public ShipPart Target;
            public Fixture Fixture;
            public Vector2 P1;
            public Vector2 P2;
            public Single Fraction;
            public GameTime GameTime;
        }

        public class CollisionDataResult
        {
            public CollisionData Data;
            public ShipPartAmmunitionCollisionResult Result;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The id of the chain that is responsible
        /// for firing this ammunition. Used to ignore
        /// internal collisions.
        /// </summary>
        public Guid ShooterChainId { get; set; }

        /// <summary>
        /// The maximum allowed age for this bullet in seconds.
        /// Once this is surpassed the bullet will be removed.
        /// </summary>
        public Double MaxAge { get; set; } = 3f;

        /// <summary>
        /// The bullets current age, used to determin when to self delete.
        /// </summary>
        public Double Age { get; private set; }

        /// <summary>
        /// Determins how much energy this piece of ammo should
        /// cost to be deflected by a shield.
        /// </summary>
        public float ShieldEnergyCost { get; set; }
        #endregion

        #region Events
        public OnEventDelegate<Ammunition, CollisionDataResult> OnCollision;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _validCollisions = new List<CollisionDataResult>();

            this.LayerGroup = VHR.LayersContexts.Ammunition.Group.GetValue();
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
            _validCollisions = null;

            base.Release();
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
            _validCollisions.Clear();
            _currentGameTime = gameTime;

            _world.Live.RayCast(
                this.HandleCollision,
                start,
                end);

            if (_validCollisions.Any())
            {
                foreach(CollisionDataResult collision in _validCollisions.OrderBy(cdr => cdr.Data.Fraction))
                {
                    this.OnCollision?.Invoke(this, collision);

                    if ((collision.Result & ShipPartAmmunitionCollisionResult.Stop) != 0)
                        break;
                }
            }
        }

        /// <summary>
        /// Calculate the cost of a blocking this projectile with an energy
        /// shield. Since this can be a static number (like with projectiles)
        /// or fluctuate based on time (like with lasers).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public abstract Single GetShieldEnergyCost(GameTime gameTime);
        #endregion

        #region Event Handlers
        private float HandleCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float fraction)
        {
            if (arg1.Tag is ShipPart target)
            {
                var collisionData = new CollisionData()
                {
                    Ammunition = this,
                    Target = target,
                    Fixture = arg1,
                    P1 = arg2,
                    P2 = arg3,
                    Fraction = fraction,
                    GameTime = _currentGameTime
                };

                var collisionDataResult = new CollisionDataResult()
                {
                    Data = collisionData,
                    Result = collisionData.Target.GetAmmunitionCollisionResult(collisionData)
                };

                if (collisionDataResult.Result != ShipPartAmmunitionCollisionResult.None)
                    _validCollisions.Add(collisionDataResult);
            }

            return 1f;
        }
        #endregion
    }
}
