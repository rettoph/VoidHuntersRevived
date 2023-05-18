using Guppy.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationEvent<TSimulationEventData> : Message<ISimulationEvent<TSimulationEventData>>, ISimulationEvent<TSimulationEventData>
        where TSimulationEventData : class
    {
        public required ParallelKey Key { get; init; }

        public int SenderId { get; init; }

        public required ISimulation Simulation { get; init; }

        public required TSimulationEventData Body { get; init; }

        public object? Response { get; private set; }

        object ISimulationEvent.Body => this.Body;

        public ParallelKey NewKey()
        {
            return this.Simulation.Keys.Next(this.Key);
        }

        public ParallelKey PreviousKey()
        {
            return this.Simulation.Keys.Previous(this.Key);
        }

        public void Respond(object? response)
        {
            this.Response = response;
        }
    }
}
