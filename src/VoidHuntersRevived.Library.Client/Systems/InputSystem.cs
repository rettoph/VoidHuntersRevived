using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Messages;
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Constants;

namespace VoidHuntersRevived.Library.Client.Systems
{
    internal sealed class InputSystem : BasicSystem,
        ISubscriber<SetPilotingDirection>
    {
        private readonly NetScope _netScope;
        private readonly ISimulationService _simulations;

        public InputSystem(
            NetScope netScope,
            ISimulationService simulations)
        {
            _netScope = netScope;
            _simulations = simulations;
        }

        public void Process(in SetPilotingDirection message)
        {
            if(_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            _simulations.PublishEvent(PeerType.Client, new SetPilotingDirection(
                pilotKey: ParallelKey.From(ParallelTypes.Pilot, _netScope.Peer.Users.Current.Id),
                which: message.Which,
                value: message.Value));
        }
    }
}
