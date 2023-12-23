using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEngineService : IDisposable
    {
        void Initialize();

        IEnumerable<T> OfType<T>();

        T Get<T>();

        IEnumerable<IEngine> All();

        void Step(Step step);
    }
}
