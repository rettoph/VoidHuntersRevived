namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IEntityService
    {
        Guid Create(EntityType type, Guid id);
        Guid Create(EntityType type, Guid id, Action<IEntityInitializer> initializer);

        void Destroy(Guid id);
    }
}
