using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IEventSystem : IEngine
    {
    }

    public interface IEventEngine<T> : IEventSystem
        where T : IEventData
    {
        void Process(VhId eventId, T data);
    }
}