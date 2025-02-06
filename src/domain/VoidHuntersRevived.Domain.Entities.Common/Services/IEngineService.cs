using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEngineService : IEnumerable<IEngine>, IDisposable
    {
        void Initialize();

        [Obsolete]
        T Get<T>();
    }
}