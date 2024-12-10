using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId);
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, GlobalEntityId globalId);
        void Despawn(VhId sourceId, EntityId id)
        {
            this.Despawn(sourceId, new GlobalEntityId(id.VhId));
        }
    }
}
