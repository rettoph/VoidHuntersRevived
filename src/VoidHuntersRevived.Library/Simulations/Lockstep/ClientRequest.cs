using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Lockstep
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(ISimulationData data)
        {
            this.Data = data;
        }

        public ISimulationData Data { get; }
    }
}
