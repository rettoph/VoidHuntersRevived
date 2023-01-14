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
    internal sealed class ServerLockstepEventService : ILockstepEventService,
        ISubscriber<INetIncomingMessage<ClientRequest>>
    {
        private readonly ITickFactory _factory;
        private Action<IData, DataSource> _publisher;

        public ServerLockstepEventService(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance ?? throw new ArgumentNullException();
            _publisher = default!;
        }

        public void Initialize(Action<IData, DataSource> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(IData data, DataSource source)
        {
            // If we are confident about the event we can publish it now
            if (source == DataSource.Determined)
            {
                _publisher.Invoke(data, source);

                return;
            }

            // events of other sources should be enqueued
            // to be confidently published later
            _factory.Enqueue(data);
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            // ClientRequest messages can be added to the factory
            // to be processed next tick
            _factory.Enqueue(message.Body.Data);
        }
    }
}
