using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class CurrentUserPlayerTractorBeamRemoteSlaveComponent : CurrentUserPlayerShipBaseComponent<TractorBeamComponent>
    {
        protected override void AddNewShipComponent(TractorBeamComponent tractorBeam)
        {
            tractorBeam.OnStateChanged += this.HandleTractorBeamStateChagned;
        }

        protected override void RemoveOldShipComponent(TractorBeamComponent tractorBeam)
        {
            tractorBeam.OnStateChanged -= this.HandleTractorBeamStateChagned;
        }

        private void HandleTractorBeamStateChagned(TractorBeamComponent sender, TractorBeamState old, TractorBeamState value)
        {
            if(value.Type == TractorBeamStateType.None)
            { // No action happened, nothing to broadcast
                return;
            }

            if(value.RequestHost == HostType.Remote)
            { // The request already came fromt he server. no need to send it back
                return;
            }

            // We should broadcast this request to the server (and coresponding RemoteMaster component will respond)
            this.Entity.SendMessage<UserPlayerTractorBeamStateRequestMessage>(new UserPlayerTractorBeamStateRequestMessage()
            {
                Type = value.Type,
                TargetPartNetworkId = value.TargetPart?.NetworkId,
                DestinationNodeNetworkId = value.DestinationNode?.NetworkId
            });
        }
    }
}
