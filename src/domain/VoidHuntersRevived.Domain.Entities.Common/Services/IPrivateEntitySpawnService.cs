using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IPrivateEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid);
        EntityId Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, VhId vhid);
        void Despawn(VhId sourceId, EntityId id)
        {
            this.Despawn(sourceId, id.VhId);
        }
    }
}
