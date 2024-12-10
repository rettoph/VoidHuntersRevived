using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IPrivateEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId);
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, EntityGlobalId globalId);
        void Despawn(VhId sourceId, EntityId id)
        {
            this.Despawn(sourceId, new EntityGlobalId(id.VhId));
        }
    }
}
