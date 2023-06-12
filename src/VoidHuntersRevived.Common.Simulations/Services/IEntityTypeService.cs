namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IEntityTypeService
    {
        void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration);
    }
}
