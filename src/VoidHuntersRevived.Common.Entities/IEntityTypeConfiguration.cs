using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        IEntityTypeConfiguration Inherits(EntityType baseType);

        IEntityTypeConfiguration Has<T>()
            where T : unmanaged, IEntityComponent;

        IEntityTypeConfiguration Has(Type component);
    }
}
