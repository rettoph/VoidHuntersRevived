using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEngineService : IDisposable
    {
        void Initialize(IStrategy strategy);

        IEnumerable<T> OfType<T>();

        T Get<T>();

        IEnumerable<IEngine> All();

        void Step(Step step);
    }
}
