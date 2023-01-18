using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(ParallelKey user, IData data)
        {
            User = user;
            Data = data;
        }

        public ParallelKey User { get; }
        public IData Data { get; }
    }
}
