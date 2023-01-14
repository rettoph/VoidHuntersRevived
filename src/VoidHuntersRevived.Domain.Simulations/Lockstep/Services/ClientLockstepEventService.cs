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
        private Action<IData, DataSource> _publisher;
        private NetScope _netScope;
        private ISimulation _predictive;
        private ISimulationService _simulations;

        public ClientLockstepEventService(NetScope netScope, ISimulationService simulations)
        {
            _publisher = default!;
            _netScope = netScope;
            _predictive = default!;
            _simulations = simulations;
        }

        public void Initialize(Action<IData, DataSource> publisher)
        {
            _publisher = publisher;
            _predictive = _simulations[SimulationType.Predictive];
        }

        public void Publish(IData data, DataSource source)
        {
            // If we are not very confident about the event 
            // we should send a request to the server
            if (source == DataSource.External)
            {
                this.RequestEvent(data);
                return;
            }

            // Any other event we *should* be able to trust
            // and publish immidiately
            _publisher.Invoke(data, source);
            _predictive.PublishEvent(data, source);
        }

        private void RequestEvent(IData data)
        {
            var message =_netScope.Messages.Create<ClientRequest>(new(data));
            message.Enqueue();
        }
    }
}
