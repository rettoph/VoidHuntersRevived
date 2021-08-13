using Guppy.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    internal class ShipBaseCRUDComponent : RemoteHostComponent<Ship>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(Constants.Messages.Ship.PlayerChanged, Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }

        protected override void Release()
        {
            base.Release();
        }
        #endregion
    }
}
