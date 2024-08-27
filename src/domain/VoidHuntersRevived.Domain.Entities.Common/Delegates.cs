using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void EntityInitializerDelegate(IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer);
}
