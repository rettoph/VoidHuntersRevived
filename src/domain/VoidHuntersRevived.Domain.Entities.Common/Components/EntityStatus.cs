using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct EntityStatus(EntityStatusEnum value) : IEntityComponent
    {
        public EntityStatusEnum Value = value;
        public int Count { get; private set; }

        public readonly bool IsSpawned => this.Count >= 1;
        public readonly bool IsDespawned => this.Count < 1;

        public bool IncrementSoftSpawnCount()
        {
            return ++this.Count == 1;
        }

        public bool IncrementSoftDespawnCount()
        {
            return --this.Count == 0;
        }
    }
}