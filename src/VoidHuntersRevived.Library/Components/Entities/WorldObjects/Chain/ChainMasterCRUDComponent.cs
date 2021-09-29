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
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ChainMasterCRUDComponent : ChainBaseCRUDComponent
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
        }

        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.log.Info($"Setting {this.Id} ({this.Entity.Root.Id})");

            this.Entity.Root.PostTreeClean += this.HandleChainRootPostTreeClean;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.log.Info($"Releasing {this.Id} ({this.Entity.Root.Id})");

            this.Entity.Root.PostTreeClean -= this.HandleChainRootPostTreeClean;
        }

        protected override void PostReleaseRemote(NetworkAuthorization authorization)
        {
            base.PostReleaseRemote(authorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;
        }
        #endregion

        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            this.Entity.WriteAll(om, this.shipPartService);
        }

        /// <summary>
        /// Mirror of <see cref="ChainSlaveCRUDComponent.ReadShipPartAttachedMessage"/>
        /// </summary>
        /// <param name="shipPart"></param>
        /// <param name="om"></param>
        private void WriteShipPartAttachedMessage(ShipPart shipPart, NetOutgoingMessage om)
        {
            // Write parent data...
            om.Write(shipPart.ChildConnectionNode.Connection.Target.Owner.Id);
            om.Write(shipPart.ChildConnectionNode.Connection.Target.Index);

            // Write child data...
            this.shipPartService.WriteShipPart(shipPart, om, ShipPartSerializationFlags.CreateTree);
            om.Write(shipPart.ChildConnectionNode.Index);
        }
        #endregion

        #region Event Handlers
        private void HandleChainRootPostTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            this.log.Info($"Broadcasting {this.Id} ({this.Entity.Root?.Id})");

            if ((components & TreeComponent.Parent) != 0)
            {
                if(source.IsRoot)
                { // The source was REMOVED from the chain
                    throw new NotImplementedException();
                }
                else if(!source.IsRoot && source.Chain == this.Entity)
                { // The source was ADDED to the chain
                    this.WriteShipPartAttachedMessage(
                        shipPart: source, 
                        om: this.Entity.Messages[Constants.Messages.Chain.ShipPartAttached].Create(this.Entity.Pipe));
                }
            }
        }
        #endregion
    }
}
