using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class TractorBeamRemoteMasterComponent : ReferenceComponent<Ship, TractorBeamComponent>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Reference.OnStateChanged += this.HandleTractorBeamStateChanged;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Reference.OnStateChanged -= this.HandleTractorBeamStateChanged;
        }

        private void HandleTractorBeamStateChanged(TractorBeamComponent sender, TractorBeamState old, TractorBeamState value)
        {
            // Broadcast a notification to all connected clients...
            this.Entity.SendMessage<ShipTractorBeamStateChangedMessage>(new ShipTractorBeamStateChangedMessage()
            {
                Type = value.Type,
                TargetPart = value.TargetPart is null ? null : new ShipPartPacket(value.TargetPart, ShipPartSerializationFlags.CreateTree),
                DestinationNodeNetworkId = value.DestinationNode?.NetworkId
            });
        }
    }
}
