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
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Library.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientLockstepEventPublishingService : ILockstepEventPublishingService
    {
        private Action<SimulationType, ISimulationData> _publisher;
        private NetScope _netScope;

        public ClientLockstepEventPublishingService(NetScope netScope)
        {
            _publisher = default!;
            _netScope = netScope;
        }

        public void Initialize(Action<SimulationType, ISimulationData> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(SimulationType source, ISimulationData data)
        {
            // Any data sourced fromm the lockstep simulation is assumed to be
            // trustworthy.
            if (source == SimulationType.Lockstep)
            {
                _publisher.Invoke(source, data);
                
                return;
            }

            // If the data came from another source we should
            // request it directly from the server.
            this.RequestEvent(data);
        }

        private void RequestEvent(ISimulationData data)
        {
            var message =_netScope.Messages.Create<ClientRequest>(new(data));
            message.Enqueue();
        }
    }
}
