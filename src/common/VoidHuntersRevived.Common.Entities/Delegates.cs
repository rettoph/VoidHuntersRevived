using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void InstanceEntityInitializerDelegate(IEntityType type, ref EntityInitializer initializer, in EntityId id);
    public delegate void StaticEntityInitializerDelegate(IEntityType type, ref EntityInitializer initializer);
    public delegate void DisposeEntityInitializerDelegate(IEntityType type);
}
