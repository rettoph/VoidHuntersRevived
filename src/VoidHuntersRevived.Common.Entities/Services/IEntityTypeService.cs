namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeService : IEntityResourceService<IEntityType>
    {
        void Register(params IEntityType[] types);
        void Configure(IEntityType type, Action<IEntityTypeConfiguration> configuration);
        IEnumerable<IEntityTypeConfiguration> GetAllConfigurations();
    }
}
