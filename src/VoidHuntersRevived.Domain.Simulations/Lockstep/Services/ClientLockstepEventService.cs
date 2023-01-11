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
    internal sealed class ClientLockstepEventService : ILockstepEventService
    {
        private Action<ISimulationData, Confidence> _publisher;
        private NetScope _netScope;

        public ClientLockstepEventService(NetScope netScope, ISimulationService simulations)
        {
            _publisher = default!;
            _netScope = netScope;
        }

        public void Initialize(Action<ISimulationData, Confidence> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(ISimulationData data, Confidence type)
        {
            // If we are not very confident about the event 
            // we should send a request to the server
            if (type == Confidence.Stochastic)
            {
                this.RequestEvent(data);
                return;
            }

            // Any other event we *should* be able to trust
            // and publish immidiately
            _publisher.Invoke(data, type);
        }

        private void RequestEvent(ISimulationData data)
        {
            var message =_netScope.Messages.Create<ClientRequest>(new(data));
            message.Enqueue();
        }
    }
}
