using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEngineService : IEnumerable<IEngine>, IDisposable
    {
        void Initialize(IStrategy strategy);

        T Get<T>();
    }
}
