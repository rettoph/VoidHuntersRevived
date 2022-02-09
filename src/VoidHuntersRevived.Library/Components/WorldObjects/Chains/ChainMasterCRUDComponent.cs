using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Network.Utilities;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    [HostTypeRequired(HostType.Remote)]
    internal sealed class ChainMasterCRUDComponent : ChainBaseCRUDComponent,
        IDataFactory<ShipPartPacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterPacket<ShipPartPacket, CreateNetworkEntityMessage>(this);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            //this.Entity.Root.PostTreeClean += this.HandleChainRootPostTreeClean;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            //this.Entity.Root.PostTreeClean -= this.HandleChainRootPostTreeClean;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterPacket<ShipPartPacket, CreateNetworkEntityMessage>(this);
        }
        #endregion

        #region Network Methods
        ShipPartPacket IDataFactory<ShipPartPacket>.Create()
        {
            return new ShipPartPacket(this.Entity.Root, ShipPartSerializationFlags.CreateTree);
        }
        #endregion

        #region Event Handlers
        private void HandleChainRootPostTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            throw new NotImplementedException();

            if ((components & TreeComponent.Parent) != 0)
            {
                if(source.IsRoot)
                { // The source was REMOVED from the chain
                    //throw new NotImplementedException();
                }
                else if(!source.IsRoot && source.Root == this.Entity.Root)
                { // The source was ADDED to the chain
                    // this.Entity.Messages[Messages.Chain.ShipPartAttached].Create(om =>
                    // {
                    //     this.WriteShipPartAttachedMessage(
                    //         shipPart: source,
                    //         om: om);
                    // }, this.Entity.Pipe);
                }
            }
        }
        #endregion
    }
}
