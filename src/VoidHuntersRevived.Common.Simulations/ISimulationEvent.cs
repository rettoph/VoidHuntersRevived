namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEvent
    {
        ParallelKey Key { get; }

        int SenderId { get; }

        ISimulation Simulation { get; }

        object Body { get; }

        ParallelKey NewKey();

        ParallelKey PreviousKey();
    }

    public interface ISimulationEvent<TSimulationEventData> : ISimulationEvent
    {
        new TSimulationEventData Body { get; }
    }
}
