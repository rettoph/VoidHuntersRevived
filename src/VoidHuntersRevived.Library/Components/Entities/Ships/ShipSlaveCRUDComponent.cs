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
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipSlaveCRUDComponent : ShipBaseCRUDComponent
    {
        #region Private Fields
        private PlayerService _players;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Constants.Messages.Ship.PlayerChanged].OnRead += this.ReadPlayerChangedMessage;
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead -= this.ReadCreateMessage;
            this.Entity.Messages[Constants.Messages.Ship.PlayerChanged].OnRead -= this.ReadPlayerChangedMessage;
        }
        #endregion

        #region Network Methods
        private void ReadCreateMessage(MessageTypeManager sender, NetIncomingMessage om)
        {
            this.Entity.Messages[Constants.Messages.Ship.PlayerChanged].TryRead(om);
        }

        private void ReadPlayerChangedMessage(MessageTypeManager sender, NetIncomingMessage om)
        {
            if(om.ReadExists())
            {
                this.Entity.Player = _players.GetById(om.ReadGuid());
            }
        }
        #endregion
    }
}
