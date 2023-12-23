namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface IRevertEventEngine : IEventEngine
    {
    }

    public interface IRevertEventEngine<T> : IRevertEventEngine
        where T : IEventData
    {
        void Revert(VhId eventId, T data);
    }
}
