namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IEventSubscriber<T>
        where T : class, IEventData
    {
        void Process(Guid id, T data);
    }
}
