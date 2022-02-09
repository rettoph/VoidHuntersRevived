using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ChainSlaveCRUDComponent : ChainBaseCRUDComponent,
        IDataProcessor<ShipPartPacket>
    {
        #region Private Fields
        private ShipPartService _shipParts;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterProcessor<ShipPartPacket>(this);

            provider.Service(out _shipParts);
            // this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            // this.Entity.Messages[Messages.Chain.ShipPartAttached].OnRead += this.ReadShipPartAttachedMessage;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterProcessor<ShipPartPacket>(this);
            // this.Entity.Messages[Messages.Chain.ShipPartAttached].OnRead -= this.ReadShipPartAttachedMessage;
            // this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead -= this.ReadCreateMessage;
        }
        #endregion

        #region Network Methods
        Boolean IDataProcessor<ShipPartPacket>.Process(ShipPartPacket message)
        {
            if(_shipParts.TryGet(message, out ShipPart root))
            {
                this.Entity.Root = root;
                return true;
            }

            return false;
        }
        // private void ReadCreateMessage(MessageTypeManager<UInt32> sender, NetIncomingMessage im)
        // {
        //     this.Entity.ReadAll(im, shipPartService);
        // }
        // 
        // /// <summary>
        // /// Mirror of <see cref="ChainMasterCRUDComponent.WriteShipPartAttachedMessage"/>
        // /// </summary>
        // /// <param name="sender"></param>
        // /// <param name="im"></param>
        // private void ReadShipPartAttachedMessage(MessageTypeManager<UInt32> sender, NetIncomingMessage im)
        // {
        //     // Load parent data
        //     ShipPart parent = this.shipPartService.GetById(im.ReadGuid());
        //     Int32 parentNodeIndex = im.ReadInt32();
        //     ConnectionNode parentNode = parent.ConnectionNodes[parentNodeIndex];
        // 
        //     // Load child data
        //     ShipPart child = this.shipPartService.TryReadShipPart(im, Enums.ShipPartSerializationFlags.CreateTree);
        //     Int32 childNodeIndex = im.ReadInt32();
        //     ConnectionNode childNode = child.ConnectionNodes[childNodeIndex];
        // 
        //     // Connect the nodes
        //     parentNode.TryAttach(childNode);
        // }
        #endregion
    }
}
