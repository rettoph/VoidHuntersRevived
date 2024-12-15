using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId);
        EntityId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, EntityGlobalId globalId);
        void Despawn(VhId sourceId, EntityLocalId localId);
        void Despawn(VhId sourceId, EntityId id)
        {
            this.Despawn(sourceId, new EntityGlobalId(id.VhId));
        }
    }
}
