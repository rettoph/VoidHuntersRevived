using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(ISimulationData data)
        {
            Data = data;
        }

        public ISimulationData Data { get; }
    }
}
