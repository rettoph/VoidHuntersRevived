using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Threading.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ShipMasterCRUDComponent : ShipBaseCRUDComponent,
        IDataFactory<ShipCreatePacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterPacket<ShipCreatePacket, CreateNetworkEntityMessage>(this);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.OnPlayerChanged += this.HandleShipPlayerChanged;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.OnPlayerChanged -= this.HandleShipPlayerChanged;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterPacket<ShipCreatePacket, CreateNetworkEntityMessage>(this);
        }
        #endregion


        #region Messege Factories
        ShipCreatePacket IDataFactory<ShipCreatePacket>.Create()
        {
            return new ShipCreatePacket()
            {
                ChainNetworkId = this.Entity.Chain.NetworkId,
                PlayerNetworkId = this.Entity.Player?.NetworkId
            };
        }
        #endregion

        #region Event Handlers
        private void HandleShipPlayerChanged(Ship sender, Player old, Player value)
        {
            this.Entity.SendMessage(new ShipPlayerChangedMessage()
            {
                PlayerNetworkId = this.Entity.Player?.NetworkId
            });
        }
        #endregion
    }
}
