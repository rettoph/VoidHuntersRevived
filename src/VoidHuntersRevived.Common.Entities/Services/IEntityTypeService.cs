namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeService
    {
        void Register(params EntityType[] types);
        void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration);
        IEnumerable<IEntityTypeConfiguration> GetAllConfigurations();
    }
}
