using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public delegate void InstanceEntityInitializerDelegate(ref EntityInitializer initializer, in EntityId id);
    public delegate void StaticEntityInitializerDelegate(ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate();
}
