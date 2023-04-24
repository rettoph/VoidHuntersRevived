using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInput : IMessage
    {
        ParallelKey Sender { get; }
        SimulationType Source { get; }
        ISimulation Simulation { get; }
        IData Data { get; }
    }

    public interface IInput<TData> : IInput
        where TData : notnull, IData
    {
        new TData Data { get; }
    }
}
