using Svelto.ECS;

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
