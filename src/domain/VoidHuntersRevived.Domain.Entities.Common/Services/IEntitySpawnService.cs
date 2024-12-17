using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySpawnService
    {
        EntityLocalId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId);
        EntityLocalId Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, EntityGlobalId globalId);
        void Despawn(VhId sourceId, EntityLocalId localId);
    }
}
