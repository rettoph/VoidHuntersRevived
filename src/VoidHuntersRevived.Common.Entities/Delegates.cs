using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void EntityInitializerDelegate(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
}
