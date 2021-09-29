using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Lidgren;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipTractorBeamSlaveCRUDComponent : ShipTractorBeamComponent
    {
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Constants.Messages.Ship.TractorBeamAction].OnRead += this.ReadShipTractorBeamActionMessage;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.Messages[Constants.Messages.Ship.TractorBeamAction].OnRead -= this.ReadShipTractorBeamActionMessage;
        }

        private void ReadShipTractorBeamActionMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Components.Get<ShipTargetingComponent>().Target = im.ReadVector2();

            TractorBeamAction action = im.ReadTractorBeamAction(this.shipParts, ShipPartSerializationFlags.None);
            this.TryAction(action);
        }
    }
}
