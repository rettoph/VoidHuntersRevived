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
using VoidHuntersRevived.Library.Simulations.Lockstep.Messages;
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
            // Any data sourced fromm the lockstep simulation is assumed to be
            // trustworthy.
            if (source == SimulationType.Lockstep)
            {
                _publisher.Invoke(source, data);

                return;
            }

            // If data came from a source other than lockstep
            // we should enqueue it into the current tick to
            // be properly processed down the line.
            _factory.Enqueue(data);
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            _factory.Enqueue(message.Body.Data);
        }
    }
}
