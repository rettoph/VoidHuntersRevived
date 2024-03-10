namespace VoidHuntersRevived.Domain.Entities.Common.Enums
{
    public enum EntityStatusEnum
    {
        NotSpawned = 0,
        HardSpawned = 1,
        SoftSpawned = 2,
        RevertSpawnEnqueued = 3,
        SoftDespawnEnqueued = 4,
        SoftDespawned = 5,
        HardDespawned = 6
    }
}
