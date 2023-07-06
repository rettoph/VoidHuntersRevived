using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Loaders
{
    [Service<IEntityTypeLoader>(ServiceLifetime.Singleton, true)]
    public interface IEntityTypeLoader
    {
        void Configure(IEntityTypeService entityTypes);
    }
}
