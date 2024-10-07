using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities
{
    internal unsafe struct EntityModificationRequest(
        VhId sourceId,
        EntityModificationType modificationType,
        in EntityId id)
    {
        public readonly VhId SourceEventId = sourceId;
        public readonly EntityModificationType ModificationType = modificationType;
        public readonly EntityId Id = id;
    }

    internal enum EntityModificationType
    {
        SoftSpawn,
        SoftDespawn,
        RevertSoftDespawn,
        HardDespawn
    }
}
