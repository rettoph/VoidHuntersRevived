using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void EntityInitializerDelegate(IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate(IEntityType type);
}
