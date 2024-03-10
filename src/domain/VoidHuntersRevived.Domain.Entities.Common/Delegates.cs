using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void InstanceEntityInitializerDelegate(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
    public delegate void StaticEntityInitializerDelegate(ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate();
}
