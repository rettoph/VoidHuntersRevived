using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Systems
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
