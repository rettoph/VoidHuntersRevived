using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IRevertEventSystem : IEventSystem
    {
    }

    public interface IRevertEventEngine<T> : IRevertEventSystem
        where T : IEventData
    {
        void Revert(VhId eventId, T data);
    }
}