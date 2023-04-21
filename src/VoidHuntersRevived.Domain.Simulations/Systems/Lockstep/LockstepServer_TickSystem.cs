using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepServer_TickSystem : BasicSystem,
        ISubscriber<Tick>
    {
        private readonly NetScope _scope;

        public LockstepServer_TickSystem(NetScope scope)
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
