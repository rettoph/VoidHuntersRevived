using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void InstanceEntityInitializerDelegate(ref EntityInitializer initializer, in EntityId id);
    public delegate void StaticEntityInitializerDelegate(ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate();
}
