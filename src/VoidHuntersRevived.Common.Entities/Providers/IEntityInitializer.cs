using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Providers
{
    [Service<IEntityInitializer>(ServiceLifetime.Scoped, true)]
    public interface IEntityInitializer
    {
        void Initialize(IEntityTypeInitializerBuilderService builder);
    }
}
