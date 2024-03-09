using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Providers
{
    [Service<IEntityInitializerProvider>(ServiceLifetime.Scoped, true)]
    public interface IEntityInitializerProvider
    {
        void Initialize(IEntityTypeInitializerBuilderService builder);
    }
}
