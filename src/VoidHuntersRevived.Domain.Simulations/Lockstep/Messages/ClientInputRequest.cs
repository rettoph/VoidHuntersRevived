using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class ClientInputRequest : Message<ClientInputRequest>
    {
        public ParallelKey Pilot { get; }
        public IData Input { get; }

        public ClientInputRequest(ParallelKey pilot, IData data)
        {
            this.Pilot = pilot;
            this.Input = data;
        }
    }
}
