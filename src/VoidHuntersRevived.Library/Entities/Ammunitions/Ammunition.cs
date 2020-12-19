using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public abstract class Ammunition : Entity
    {
        #region Private Fields
        private WorldEntity _world;
        #endregion

        #region Public Properties
        /// <summary>
        /// The id of the chain that is responsible
        /// for firing this ammunition. Used to ignore
        /// internal collisions.
        /// </summary>
        public Guid ShooterId { get; set; }
        #endregion

        #region Events
        public OnEventDelegate<Ammunition, ShipPart> OnCollision;
        public ValidateEventDelegate<Ammunition, ShipPart> ValidateCollision;
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
        }
        protected override void Dispose()
        {
            base.Dispose();

            this.ValidateCollision -= this.HandleValidateCollision;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Check all collisions between the given start and end position,
        /// if any. Handle any found collisions as needed.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void CheckCollisions(Vector2 start, Vector2 end)
        {
            _world.Live.RayCast(
                this.HandleCollision,
                start,
                end);
        }
        #endregion

        #region Event Handlers
        private float HandleCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float arg4)
        {
            if (arg1.UserData is ShipPart target && this.ValidateCollision.Validate(this, target, true))
            {
                this.OnCollision?.Invoke(this, target);

                return 0;
            }

            return -1;
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
        private bool HandleValidateCollision(Ammunition sender, ShipPart shipPart)
            => shipPart.Chain.Id != this.ShooterId && shipPart.Health > 0 && shipPart.Chain.Ship != default;
        #endregion
    }
}
