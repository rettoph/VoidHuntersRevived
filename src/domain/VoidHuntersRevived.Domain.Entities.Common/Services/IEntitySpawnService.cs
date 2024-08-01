using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySpawnService
    {
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, bool isPrivate = false);
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, EntityInitializerDelegate initializer, bool isPrivate = false);

        void Despawn(VhId sourceId, VhId vhid, bool isPrivate = false);
        void Despawn(VhId sourceId, EntityId id, bool isPrivate = false);
    }
}
