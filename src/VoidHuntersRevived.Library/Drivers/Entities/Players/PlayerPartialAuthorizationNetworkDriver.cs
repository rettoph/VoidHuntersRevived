using Guppy;
using Guppy.Collections;
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
    internal sealed class PlayerPartialAuthorizationNetworkDriver : BaseAuthorizationDriver<Player>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigurePartial(ServiceProvider provider)
        {
            base.ConfigurePartial(provider);

            provider.Service(out _entities);

            this.driven.Actions.Set("update:ship", this.ReadShip);
        }

        protected override void DisposePartial()
        {
            base.DisposePartial();

            this.driven.Actions.Remove("update:ship");
        }
        #endregion

        #region Network Methods
        private void ReadShip(NetIncomingMessage obj)
            => this.driven.Ship = obj.ReadEntity<Ship>(_entities);
        #endregion
    }
}
