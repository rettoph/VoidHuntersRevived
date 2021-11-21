using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Lists;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipSlaveCRUDComponent : ShipBaseCRUDComponent
    {
        #region Private Fields
        private PlayerService _players;
        private NetworkEntityList _networkEntities;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            provider.Service(out _players);
            provider.Service(out _networkEntities);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Messages.Ship.PlayerChanged].OnRead += this.ReadPlayerChangedMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            _players = default;
            _networkEntities = default;

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead -= this.ReadCreateMessage;
            this.Entity.Messages[Messages.Ship.PlayerChanged].OnRead -= this.ReadPlayerChangedMessage;
        }
        #endregion

        #region Network Methods
        private void ReadCreateMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Chain = _networkEntities.GetById<Chain>(im.ReadGuid());
            this.Entity.Messages[Messages.Ship.PlayerChanged].TryRead(im);
        }

        private void ReadPlayerChangedMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            if(im.ReadExists())
            {
                this.Entity.Player = _players.GetById(im.ReadGuid());
            }
        }
        #endregion
    }
}
