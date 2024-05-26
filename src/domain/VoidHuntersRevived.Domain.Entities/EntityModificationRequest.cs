using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities
{
    internal unsafe struct EntityModificationRequest
    {
        public readonly VhId SourceEventId;
        public readonly EntityModificationType ModificationType;
        public readonly EntityId Id;

        public EntityModificationRequest(
            VhId sourceId,
            EntityModificationType modificationType,
            in EntityId id)
        {
            SourceEventId = sourceId;
            ModificationType = modificationType;
            Id = id;
        }
    }

    internal enum EntityModificationType
    {
        SoftSpawn,
        SoftDespawn,
        RevertSoftDespawn,
        HardDespawn
    }
}
