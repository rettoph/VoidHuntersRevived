using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Messages;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Constants;

namespace VoidHuntersRevived.Domain.Client.Systems
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

            _simulations.Input(
                confidence: Confidence.Unknown, 
                data: new SetPilotingDirection(
                    pilotKey: ParallelKey.From(ParallelTypes.Pilot, _netScope.Peer.Users.Current.Id),
                    which: message.Which,
                    value: message.Value));
        }
    }
}
