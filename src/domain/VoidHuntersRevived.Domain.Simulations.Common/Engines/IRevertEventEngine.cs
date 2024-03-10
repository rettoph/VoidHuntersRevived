using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
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
