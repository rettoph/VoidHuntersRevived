using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Providers
{
    [Service<IEngineProvider>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface IEngineProvider
    {
        IEnumerable<IEngine> GetEngines();
    }
}
