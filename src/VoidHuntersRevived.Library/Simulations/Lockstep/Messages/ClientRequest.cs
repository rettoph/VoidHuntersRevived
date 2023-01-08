using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Messages
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(ISimulationInputData data)
        {
            Data = data;
        }

        public ISimulationInputData Data { get; }
    }
}
