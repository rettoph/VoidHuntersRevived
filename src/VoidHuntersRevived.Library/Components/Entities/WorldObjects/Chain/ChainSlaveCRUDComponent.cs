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
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ChainSlaveCRUDComponent : ChainBaseCRUDComponent
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Messages.Chain.ShipPartAttached].OnRead += this.ReadShipPartAttachedMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Messages[Messages.Chain.ShipPartAttached].OnRead -= this.ReadShipPartAttachedMessage;
            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead -= this.ReadCreateMessage;
        }
        #endregion

        #region Network Methods
        private void ReadCreateMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.ReadAll(im, shipPartService);
        }

        /// <summary>
        /// Mirror of <see cref="ChainMasterCRUDComponent.WriteShipPartAttachedMessage"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="im"></param>
        private void ReadShipPartAttachedMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            // Load parent data
            ShipPart parent = this.shipPartService.GetById(im.ReadGuid());
            Int32 parentNodeIndex = im.ReadInt32();
            ConnectionNode parentNode = parent.ConnectionNodes[parentNodeIndex];

            // Load child data
            ShipPart child = this.shipPartService.TryReadShipPart(im, Enums.ShipPartSerializationFlags.CreateTree);
            Int32 childNodeIndex = im.ReadInt32();
            ConnectionNode childNode = child.ConnectionNodes[childNodeIndex];

            // Connect the nodes
            parentNode.TryAttach(childNode);
        }
        #endregion
    }
}
