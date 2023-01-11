using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEvent : IMessage
    {
        Confidence Confidence { get; }
        ISimulation Simulation { get; }
        ISimulationData Data { get; }
    }

    public interface ISimulationEvent<TData> : ISimulationEvent
        where TData : notnull, ISimulationData
    {
        new TData Data { get; }
    }
}
