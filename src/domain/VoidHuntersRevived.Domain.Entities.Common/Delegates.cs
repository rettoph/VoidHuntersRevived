using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void EntityInitializerDelegate(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
}
