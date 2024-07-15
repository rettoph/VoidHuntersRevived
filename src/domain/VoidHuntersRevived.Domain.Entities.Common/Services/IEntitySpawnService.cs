using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid);
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, EntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, VhId vhid);
        void Despawn(VhId sourceId, EntityId id);
    }
}
