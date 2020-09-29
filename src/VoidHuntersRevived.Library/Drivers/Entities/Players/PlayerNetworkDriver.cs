using Guppy.Lists;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class PlayerNetworkDriver : NetworkEntityNetworkDriver<Player>
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            this.AddAction("update:ship", this.SkipShip, (GameAuthorization.Minimum, this.ReadShip));
        }

        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnShipChanged += this.HandleShipChanged;
            this.driven.OnWrite += this.WriteShip;
        }

        protected override void ReleaseFull()
        {
            base.ReleaseFull();

            this.driven.OnShipChanged -= this.HandleShipChanged;
            this.driven.OnWrite -= this.WriteShip;
        }

        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            provider.Service(out _entities);
        }
        #endregion

        #region Network Methods
        private void WriteShip(NetOutgoingMessage om)
            => om.Write("update:ship", m => m.Write(this.driven.Ship));
        #endregion

        #region Event Handlers
        private void HandleShipChanged(Player sender, Ship old, Ship value)
            => this.WriteShip(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 5));

        private void ReadShip(NetIncomingMessage obj)
            => this.driven.Ship = obj.ReadEntity<Ship>(_entities);

        private void SkipShip(NetIncomingMessage obj)
        {
            if (obj.ReadBoolean())
                obj.Position += 128;
        }
        #endregion
    }
}
