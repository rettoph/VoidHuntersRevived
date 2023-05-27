using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationEvent<TBody> : Message<ISimulationEvent<TBody>>, ISimulationEvent<TBody>
        where TBody : class
    {
        public required ParallelKey Key { get; init; }

        public int SenderId { get; init; }

        public required ISimulation Simulation { get; init; }

        public required TBody Body { get; init; }

        public object? Response { get; private set; }

        object ISimulationEvent.Body => this.Body;

        public void Respond(object? response)
        {
            this.Response = response;
        }
    }
}
