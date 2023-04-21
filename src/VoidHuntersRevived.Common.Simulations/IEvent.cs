using Guppy.Common;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEvent : IMessage
    {
        ParallelKey Sender { get; }
        SimulationType Source { get; }
        ISimulation Target { get; }
        IData Data { get; }

        void PublishConsequent(IData data);
    }

    public interface IEvent<TData> : IEvent
        where TData : notnull, IData
    {
        new TData Data { get; }
    }
}
