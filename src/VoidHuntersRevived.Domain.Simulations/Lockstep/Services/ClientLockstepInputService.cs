using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientLockstepInputService : ILockstepInputService
    {
        private NetScope _netScope;

        public ClientLockstepInputService(NetScope netScope, ISimulationService simulations)
        {
            _netScope = netScope;
        }

        public void Input(ParallelKey user, IData data)
        {
            _netScope.Messages.Create(
                body: new ClientInputRequest(
                    pilot: user, 
                    data: data)).Enqueue();
        }
    }
}
