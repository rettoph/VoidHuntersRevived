using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IEventEngine : IEngine
    {
    }

    public interface IEventEngine<T> : IEventEngine
        where T : IEventData
    {
        void Process(VhId eventId, T data);
    }
}
