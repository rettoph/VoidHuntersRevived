using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void InstanceEntityInitializerDelegate(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
    public delegate void StaticEntityInitializerDelegate(ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate();
}
