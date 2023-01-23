using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class ClientInputRequest : Message<ClientInputRequest>
    {
        public ParallelKey User { get; }
        public IData Input { get; }

        public ClientInputRequest(ParallelKey user, IData data)
        {
            this.User = user;
            this.Input = data;
        }
    }
}
