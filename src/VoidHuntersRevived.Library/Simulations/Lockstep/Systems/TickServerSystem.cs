using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Attributes;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Common;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class TickServerSystem : BasicSystem,
        ISubscriber<Tick>
    {
        private readonly NetScope _scope;

        public TickServerSystem(NetScope scope)
        {
            _scope = scope;
        }

        public void Process(in Tick message)
        {
            // Broadcast the current tick to all connected peers
            _scope.Messages.Create(in message)
                .AddRecipients(_scope.Users.Peers)
                .Enqueue();
        }
    }
}
