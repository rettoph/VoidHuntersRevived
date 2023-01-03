using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientLockstepEventPublishingService : ILockstepEventPublishingService
    {
        private Action<PeerType, ISimulationData> _publisher;

        public ClientLockstepEventPublishingService()
        {
            _publisher = default!;
        }

        public void Initialize(Action<PeerType, ISimulationData> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(PeerType source, ISimulationData data)
        {
            // If we are currently on the client and recieve a client
            // event then we should ignore it. Wait for verification from
            // the server before processing it.
            if (source == PeerType.Client)
            {
                return;
            }

            // If the source is the server, that means we are free to publish.
            _publisher.Invoke(source, data);
        }
    }
}
