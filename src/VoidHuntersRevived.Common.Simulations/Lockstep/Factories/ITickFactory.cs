namespace VoidHuntersRevived.Common.Simulations.Lockstep.Factories
{
    public interface ITickFactory
    {
        void Enqueue(EventDto @event);
        Tick Create(int id);
        void Reset();
    }
}
