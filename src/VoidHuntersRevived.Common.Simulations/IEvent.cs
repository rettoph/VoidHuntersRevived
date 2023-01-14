using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEvent : IMessage
    {
        DataSource Source { get; }
        ISimulation Simulation { get; }
        IData Data { get; }
    }

    public interface IEvent<TData> : IEvent
        where TData : notnull, IData
    {
        new TData Data { get; }
    }
}
