namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeService
    {
        void Register(params IEntityType[] types);
        void Configure(IEntityType type, Action<IEntityTypeConfiguration> configuration);
        IEnumerable<IEntityTypeConfiguration> GetAllConfigurations();
        IEntityType GetById(EntityTypeId id);
    }
}
