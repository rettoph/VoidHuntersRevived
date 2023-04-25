using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class Server_InputSystem : BasicSystem,
        ISubscriber<INetIncomingMessage<InputRequest>>
    {
        private readonly ISimulationService _simulations;

        public Server_InputSystem(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        public void Process(in INetIncomingMessage<InputRequest> message)
        {
            _simulations.Input(message.Body.Input);
        }
    }
}
