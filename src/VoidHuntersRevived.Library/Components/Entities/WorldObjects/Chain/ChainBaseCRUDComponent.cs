using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    internal class ChainBaseCRUDComponent : NetworkComponent<Chain>
    {
        #region Protected Properties Fields
        protected ShipPartService shipPartService { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.shipPartService = provider.GetService<ShipPartService>();

            this.Entity.Messages.Add(
                messageType: Constants.Messages.Chain.ShipPartAttached, 
                defaultContext: Guppy.Network.Constants.MessageContexts.InternalReliableDefault);
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.shipPartService = default;
        }
        #endregion
    }
}
