namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        IEntityTypeConfiguration Inherits(EntityType baseType);

        IEntityTypeConfiguration Has<T>()
            where T : unmanaged;
    }
}
