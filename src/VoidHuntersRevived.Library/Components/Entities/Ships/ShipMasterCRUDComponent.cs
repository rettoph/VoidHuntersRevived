using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ShipMasterCRUDComponent : ShipBaseCRUDComponent
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.OnPlayerChanged += this.HandleShipPlayerChanged;
            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
            this.Entity.Messages[Messages.Ship.PlayerChanged].OnWrite += this.WritePlayerChangedMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.OnPlayerChanged -= this.HandleShipPlayerChanged;
            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;
            this.Entity.Messages[Messages.Ship.PlayerChanged].OnWrite -= this.WritePlayerChangedMessage;
        }
        #endregion


        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            om.Write(this.Entity.Chain.Id);
            this.Entity.Messages[Messages.Ship.PlayerChanged].TryWrite(om);
        }

        private void WritePlayerChangedMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            if(om.WriteExists(this.Entity.Player))
            {
                om.Write(this.Entity.Player.Id);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleShipPlayerChanged(Ship sender, Player old, Player value)
        {
            this.Entity.Messages[Messages.Ship.PlayerChanged].Create(this.Entity.Pipe);
        }
        #endregion
    }
}
