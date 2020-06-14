using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class PlayerFullAuthorizationNetworkDriver : BaseAuthorizationDriver<Player>
    {
        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnShipChanged += this.HandleShipChanged;
            this.driven.OnWrite += this.WriteShip;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.OnShipChanged -= this.HandleShipChanged;
            this.driven.OnWrite -= this.WriteShip;
        }
        #endregion

        #region Network Methods
        private void WriteShip(NetOutgoingMessage om)
            => om.Write("update:ship", m => m.Write(this.driven.Ship));
        #endregion

        #region Event Handlers
        private void HandleShipChanged(Player sender, Ship old, Ship value)
            => this.WriteShip(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 5));
        #endregion
    }
}
