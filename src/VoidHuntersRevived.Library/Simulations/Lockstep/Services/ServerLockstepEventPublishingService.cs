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
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerLockstepEventPublishingService : ILockstepEventPublishingService,
        ISubscriber<INetIncomingMessage<ClientRequest>>
    {
        private readonly ITickFactory _factory;
        private Action<SimulationType, ISimulationData> _publisher;

        public ServerLockstepEventPublishingService(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance ?? throw new ArgumentNullException();
            _publisher = default!;
        }

        public void Initialize(Action<SimulationType, ISimulationData> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(SimulationType source, ISimulationData data)
        {
            // If we are currently on the server then all predictive sourced
            // data should be enqueued and await publishing within the
            // tick.
            if(source != SimulationType.Lockstep)
            {
                _factory.Enqueue(data);
                return;
            }

            // If the source is lockstep, that means we are free to publish.
            _publisher.Invoke(source, data);
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            _factory.Enqueue(message.Body.Data);
        }
    }
}
