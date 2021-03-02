using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Lidgren.Network;
using System;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial implementation of <see cref="ShipPart"/>
    /// that manages health and damage related actions.
    /// </summary>
    public partial class ShipPart
    {
        #region Private Fields
        /// <summary>
        /// The primary placeholder field for the
        /// <see cref="Health"/> property.
        /// </summary>
        private Single _health;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current part's health. By default this is based on the
        /// <see cref="ShipPartConfiguration.MaxHealth"/> value.
        /// </summary>
        public Single Health
        {
            get => _health;
            set => this.OnHealthChanged.InvokeIf(_health != value, this, ref _health, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<ShipPart, Single> OnHealthChanged;
        #endregion

        #region Lifecycle Methods
        private void Health_Initialize(ServiceProvider provider)
        {
            this.Health = this.Context.MaxHealth;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to apply the specified amount of damage 
        /// to the current <see cref="ShipPart"/>. The
        /// minimum health should always be 0.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TryDamage(Single damage)
            => this.Health = Math.Max(0f, this.Health - damage);

        public virtual ShipPartAmmunitionCollisionResult GetAmmunitionCollisionResult(Ammunition.CollisionData collision)
        {
            if (this.Chain.Ship == default)
                return ShipPartAmmunitionCollisionResult.None;

            if (collision.Ammunition.ShooterChainId == this.Chain.Id)
                return ShipPartAmmunitionCollisionResult.None;

            if (this.Health > 0)
                return ShipPartAmmunitionCollisionResult.DamageAndStop;

            return ShipPartAmmunitionCollisionResult.None;
        }
        #endregion

        #region Network Methods
        public void ReadHealth(NetIncomingMessage im)
            => this.Health = im.ReadSingle();

        public void WriteHealth(NetOutgoingMessage om)
            => om.Write(this.Health);
        #endregion
    }
}
