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
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class PlayerMinimumAuthorizationNetworkDriver : BaseAuthorizationDriver<Player>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            provider.Service(out _entities);

            this.driven.Actions.Set("update:ship", this.ReadShip);
        }

        protected override void DisposeMinimum()
        {
            base.DisposeMinimum();

            this.driven.Actions.Remove("update:ship");
        }
        #endregion

        #region Network Methods
        private void ReadShip(NetIncomingMessage obj)
            => this.driven.Ship = obj.ReadEntity<Ship>(_entities);
        #endregion
    }
}
