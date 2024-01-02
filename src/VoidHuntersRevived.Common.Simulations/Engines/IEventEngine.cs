using Svelto.ECS;
using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Simulations.Engines
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
