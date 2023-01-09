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
using VoidHuntersRevived.Domain.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerLockstepEventPublishingService : ILockstepEventPublishingService,
        ISubscriber<INetIncomingMessage<ClientRequest>>
    {
        private readonly ITickFactory _factory;
        private Action<ISimulationInputData, Confidence> _publisher;

        public ServerLockstepEventPublishingService(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance ?? throw new ArgumentNullException();
            _publisher = default!;
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

            // events of unknown confidence should be enqueued
            // to be confidently published later
            _factory.Enqueue(data);
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            _factory.Enqueue(message.Body.Data);
        }
    }
}
