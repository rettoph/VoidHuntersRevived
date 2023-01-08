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
        private Action<ISimulationInputData, Confidence> _publisher;
        private NetScope _netScope;

        public ClientLockstepEventPublishingService(NetScope netScope)
        {
            _publisher = default!;
            _netScope = netScope;
        }

        public void Initialize(Action<ISimulationInputData, Confidence> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(ISimulationInputData data, Confidence confidence)
        {
            // If we are confident about the event we can publish it now
            if (confidence == Confidence.Certain)
            {
                _publisher.Invoke(data, confidence);
                
                return;
            }

            // If we are not confident about the event we should
            // request it from the server
            this.RequestEvent(data);
        }

        private void RequestEvent(ISimulationInputData data)
        {
            var message =_netScope.Messages.Create<ClientRequest>(new(data));
            message.Enqueue();
        }
    }
}
