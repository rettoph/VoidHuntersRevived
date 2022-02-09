using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Services;
using Guppy.Network.Utilities;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipSlaveCRUDComponent : ShipBaseCRUDComponent,
        IDataProcessor<ShipCreatePacket>,
        IDataProcessor<ShipPlayerChangedMessage>
    {
        #region Private Fields
        private PlayerService _players;
        private NetworkEntityService _networkEntities;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _players);
            provider.Service(out _networkEntities);

            this.Entity.Messages.RegisterProcessor<ShipCreatePacket>(this);
            this.Entity.Messages.RegisterProcessor<ShipPlayerChangedMessage>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterProcessor<ShipCreatePacket>(this);
            this.Entity.Messages.DeregisterProcessor<ShipPlayerChangedMessage>(this);
        }
        #endregion

        #region Helper Methods
        private void SetChain(UInt16 chainId)
        {
            if (!_networkEntities.TryGetByNetworkId(chainId, out Chain chain))
            {
                throw new InvalidOperationException();
            }

            this.Entity.Chain = chain;
        }
        private void SetPlayer(UInt16? playerNetworkId)
        {
            if (playerNetworkId.HasValue && _networkEntities.TryGetByNetworkId(playerNetworkId.Value, out Player player))
            {
                this.Entity.Player = player;
            }
            else
            {
                this.Entity.Player = default;
            }
        }
        #endregion

        #region Message Processors
        Boolean IDataProcessor<ShipCreatePacket>.Process(ShipCreatePacket message)
        {
            this.SetChain(message.ChainNetworkId);
            this.SetPlayer(message.PlayerNetworkId);

            return true;
        }

        Boolean IDataProcessor<ShipPlayerChangedMessage>.Process(ShipPlayerChangedMessage message)
        {
            this.SetPlayer(message.PlayerNetworkId);

            return true;
        }
        #endregion
    }
}
