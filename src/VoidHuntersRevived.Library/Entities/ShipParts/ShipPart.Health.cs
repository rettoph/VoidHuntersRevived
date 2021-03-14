using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Services.Spells;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial implementation of <see cref="ShipPart"/>
    /// that manages health and damage related actions.
    /// </summary>
    public partial class ShipPart : IAmmunitionSpellTarget
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

        public event ValidateEventDelegate<IAmmunitionSpellTarget, AmmunitionSpell.CollisionData> OnValidateAmmunitionCollision;
        public event IAmmunitionSpellTarget.ApplyAmmunitionCollisionDelegate OnApplyAmmunitionCollision;
        #endregion

        #region Lifecycle Methods
        private void Health_Create(ServiceProvider provider)
        {
            this.OnValidateAmmunitionCollision += this.ValidateAmmunitionCollision;
        }

        private void Health_Initialize(ServiceProvider provider)
        {
            this.Health = this.Context.MaxHealth;
        }

        private void Health_Dispose()
        {
            this.OnValidateAmmunitionCollision -= this.ValidateAmmunitionCollision;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to apply the specified amount of damage 
        /// to the current <see cref="ShipPart"/>. The
        /// minimum health should always be 0.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TryApplyDamage(Single damage)
            => this.Health = Math.Max(0f, this.Health - damage);

        Boolean IAmmunitionSpellTarget.ValidateAmmunitionCollision(AmmunitionSpell.CollisionData data)
            => this.OnValidateAmmunitionCollision.Validate(this, data, false);

        void IAmmunitionSpellTarget.ApplyAmmunitionCollision(AmmunitionSpell.CollisionData data, GameTime gameTime)
            => this.OnApplyAmmunitionCollision?.Invoke(this, data, gameTime);
        #endregion

        #region Network Methods
        public void ReadHealth(NetIncomingMessage im)
            => this.Health = im.ReadSingle();

        public void WriteHealth(NetOutgoingMessage om)
            => om.Write(this.Health);
        #endregion

        #region Event Handlers
        /// <summary>
        /// Ensure all ship parts are attached to a ship, not fired by the current part, and have over
        /// 0 health.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool ValidateAmmunitionCollision(IAmmunitionSpellTarget sender, AmmunitionSpell.CollisionData data)
        {
            return this.Chain.Ship != default && data.Ammunition.Weapon != this && this.Health > 0;
        }
        #endregion
    }
}
