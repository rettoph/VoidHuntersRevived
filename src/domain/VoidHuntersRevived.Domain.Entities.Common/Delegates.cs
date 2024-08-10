using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void EntityInitializerDelegate(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate(IEntityTypeProvider provider);
}
