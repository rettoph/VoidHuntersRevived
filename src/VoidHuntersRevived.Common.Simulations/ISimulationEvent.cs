using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEvent : IMessage
    {
        ParallelKey Key { get; }

        int SenderId { get; }

        ISimulation Simulation { get; }

        object Body { get; }

        object? Response { get; }

        ParallelKey NewKey();

        ParallelKey PreviousKey();

        void Respond(object? response);
    }

    public interface ISimulationEvent<TBody> : ISimulationEvent
    {
        new TBody Body { get; }
    }
}
