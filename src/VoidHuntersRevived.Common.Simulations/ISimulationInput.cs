using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationInput : IMessage
    {
        Confidence Confidence { get; }
        ISimulation Simulation { get; }
        ISimulationInputData Data { get; }
    }

    public interface ISimulationInput<TData> : ISimulationInput
        where TData : notnull, ISimulationInputData
    {
        new TData Data { get; }
    }
}
