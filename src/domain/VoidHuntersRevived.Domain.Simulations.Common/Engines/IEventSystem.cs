using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IEventSystem : ISceneSystem
    {
    }

    public interface IEventSystem<T> : IEventSystem
        where T : IEventData
    {
        void Process(VhId eventId, T data);
    }
}