using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Providers
{
    public interface IEngineProvider
    {
        IEnumerable<IEngine> GetEngines();
    }
}