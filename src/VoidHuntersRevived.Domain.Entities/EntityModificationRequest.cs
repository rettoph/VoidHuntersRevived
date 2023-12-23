using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities
{
    internal unsafe struct EntityModificationRequest
    {
        public readonly EntityModificationType ModificationType;
        public readonly EntityId Id;

        public EntityModificationRequest(
            EntityModificationType modificationType,
            in EntityId id)
        {
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
