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
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerLockstepInputService : ILockstepInputService,
        ISubscriber<INetIncomingMessage<ClientRequest>>
    {
        private readonly ITickFactory _factory;

        public ServerLockstepInputService(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance ?? throw new ArgumentNullException();
        }

        public void Process(in INetIncomingMessage<ClientRequest> message)
        {
            if(message.Peer is null)
            {
                return;
            }

            this.Input(message.Peer.GetPilotKey(), message.Body.Data);
        }

        public void Input(ParallelKey user, IData data)
        {
            _factory.Enqueue(new UserInput(user, data));
        }
    }
}
