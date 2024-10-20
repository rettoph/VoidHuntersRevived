using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEngineService : IEnumerable<IEngine>, IDisposable
    {
        // TODO: Remove unused parameter
        void Initialize(IStrategy strategy);

        T Get<T>();
    }
}
