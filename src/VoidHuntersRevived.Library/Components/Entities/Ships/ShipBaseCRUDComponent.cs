using Guppy.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    internal class ShipBaseCRUDComponent : NetworkComponent<Ship>
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(Messages.Ship.PlayerChanged, Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }
        #endregion
    }
}
