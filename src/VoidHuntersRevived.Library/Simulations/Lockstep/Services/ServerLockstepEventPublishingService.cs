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
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerLockstepEventPublishingService : ILockstepEventPublishingService,
        ISubscriber<INetIncomingMessage<ClientRequest>>
    {
        private readonly ITickFactory _factory;
        private Action<PeerType, ISimulationData> _publisher;

        public ServerLockstepEventPublishingService(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance ?? throw new ArgumentNullException();
            _publisher = default!;
        }

        public void Initialize(Action<PeerType, ISimulationData> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(PeerType source, ISimulationData data)
        {
            // If we are currently on the server then all client sourced
            // data should be enqueued and await publishing within the
            // tick.
            if(source == PeerType.Client)
            {
                _factory.Enqueue(data);
                return;
            }

            // If the source is the server, that means we are free to publish.
            _publisher.Invoke(source, data);
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            _factory.Enqueue(message.Body.Data);
        }
    }
}
