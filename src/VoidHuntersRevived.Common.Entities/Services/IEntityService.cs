using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public delegate void EntityInitializerDelegate(ref EntityInitializer initializer);

    public interface IEntityService
    {
        Guid Create(EntityType type, Guid id);
        Guid Create(EntityType type, Guid id, EntityInitializerDelegate initializer);

        void Destroy(Guid id);
    }
}
