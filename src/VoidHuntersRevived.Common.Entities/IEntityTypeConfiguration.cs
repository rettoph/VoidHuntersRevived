using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        IEntityTypeConfiguration Inherits(EntityType baseType);

        IEntityTypeConfiguration HasComponent<T>()
            where T : unmanaged, IEntityComponent;

        IEntityTypeConfiguration HasComponent(Type component);

        IEntityTypeConfiguration HasProperty<T>()
            where T : IEntityProperty;
    }
}
