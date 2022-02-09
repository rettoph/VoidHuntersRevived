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
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class CurrentUserPlayerTractorBeamRemoteMasterComponent : Component<UserPlayer>,
        IDataProcessor<UserPlayerTractorBeamStateRequestMessage>
    {
        private ShipPartService _shipParts;

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _shipParts);

            this.Entity.Messages.RegisterProcessor<UserPlayerTractorBeamStateRequestMessage>(this);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Messages.DeregisterProcessor<UserPlayerTractorBeamStateRequestMessage>(this);
        }

        bool IDataProcessor<UserPlayerTractorBeamStateRequestMessage>.Process(UserPlayerTractorBeamStateRequestMessage request)
        {
            switch (request.Type)
            {
                case TractorBeamStateType.Select:
                    return this.ProcessSelectRequest(request);
                case TractorBeamStateType.Deselect:
                    return this.ProcessDeselectRequest(request);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool ProcessSelectRequest(UserPlayerTractorBeamStateRequestMessage request)
        {
            if(request.TargetPartNetworkId.HasValue && _shipParts.TryGetByNetworkId(request.TargetPartNetworkId.Value, out ShipPart targetPart))
            {
                this.Entity.Ship?.Components.Get<TractorBeamComponent>().EnqueueState(
                    requestHost: HostType.Remote,
                    type: TractorBeamStateType.Select,
                    targetPart: targetPart,
                    destinationNode: default);

                return true;
            }

            return false;
        }

        private bool ProcessDeselectRequest(UserPlayerTractorBeamStateRequestMessage request)
        {
            ShipPart targetPart = default;
            if(request.TargetPartNetworkId.HasValue)
            {
                _shipParts.TryGetByNetworkId(request.TargetPartNetworkId.Value, out targetPart);
            }

            ConnectionNode destinationNode = default;
            if(request.DestinationNodeNetworkId.HasValue && _shipParts.TryGetByNetworkId(request.DestinationNodeNetworkId.Value.OwnerNetworkId, out ShipPart destinationPart))
            {
                destinationNode = destinationPart.ConnectionNodes.ElementAtOrDefault(request.DestinationNodeNetworkId.Value.Index);
            }

            this.Entity.Ship?.Components.Get<TractorBeamComponent>().EnqueueState(
                requestHost: HostType.Remote,
                type: TractorBeamStateType.Deselect,
                destinationNode: destinationNode,
                targetPart: targetPart);

            return true;
        }
    }
}
