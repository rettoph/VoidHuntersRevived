using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void EntityInitializerDelegate(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate(IEntityType type);
}
