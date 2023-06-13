namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeService
    {
        void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration);
    }
}
