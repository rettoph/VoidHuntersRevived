using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(IData data)
        {
            Data = data;
        }

        public IData Data { get; }
    }
}
