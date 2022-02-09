using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Services;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class TractorBeamRemoteSlaveComponent : ReferenceComponent<Ship, TractorBeamComponent>,
        IDataProcessor<ShipTractorBeamStateChangedMessage>
    {
        private ShipPartService _shipParts;
        private NetworkEntityService _networkEntities;

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _shipParts);
            provider.Service(out _networkEntities);

            this.Entity.Messages.RegisterProcessor<ShipTractorBeamStateChangedMessage>(this);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Messages.DeregisterProcessor<ShipTractorBeamStateChangedMessage>(this);
        }

        bool IDataProcessor<ShipTractorBeamStateChangedMessage>.Process(ShipTractorBeamStateChangedMessage data)
        {
            _shipParts.TryGet(data.TargetPart, out ShipPart targetPart);
            _shipParts.TryGetConnectionNodeByNetworkId(data.DestinationNodeNetworkId, out ConnectionNode destinationNode);

            this.Reference.EnqueueState(HostType.Remote, data.Type, targetPart, destinationNode);

            return true;
        }
    }
}
