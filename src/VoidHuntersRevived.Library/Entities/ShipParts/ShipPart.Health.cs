using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;

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

        #region Network Methods
        public void ReadHealth(NetIncomingMessage im)
            => this.Health = im.ReadSingle();

        public void WriteHealth(NetOutgoingMessage om)
            => om.Write(this.Health);
        #endregion
    }
}
