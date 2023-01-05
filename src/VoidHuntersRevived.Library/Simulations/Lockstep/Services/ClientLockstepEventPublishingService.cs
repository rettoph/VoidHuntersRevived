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

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientLockstepEventPublishingService : ILockstepEventPublishingService
    {
        private Action<PeerType, ISimulationData> _publisher;
        private NetScope _netScope;

        public ClientLockstepEventPublishingService(NetScope netScope)
        {
            _publisher = default!;
            _netScope = netScope;
        }

        public void Initialize(Action<PeerType, ISimulationData> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(PeerType source, ISimulationData data)
        {
            // If we are currently on the client and recieve a client
            // event then we should request it from the server.
            // Wait for verification from the server before publishing it.
            if (source == PeerType.Client)
            {
                this.RequestEvent(data);
                return;
            }

            // If the source is the server, that means we are free to publish.
            _publisher.Invoke(source, data);
        }

        private void RequestEvent(ISimulationData data)
        {
            var message =_netScope.Messages.Create<ClientRequest>(new(data));
            message.Enqueue();
        }
    }
}
