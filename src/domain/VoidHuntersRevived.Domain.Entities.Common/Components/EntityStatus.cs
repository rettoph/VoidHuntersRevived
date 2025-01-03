using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct EntityStatus : IEntityComponent
    {
        private int _counter;

        public EntityStatusEnum Value;
        public int Count => _counter;

        public EntityStatus(EntityStatusEnum value)
        {
            this.Value = value;
        }

        public bool IsSpawned => _counter >= 1;
        public bool IsDespawned => _counter < 1;

        public bool IncrementSoftSpawnCount()
        {
            return ++_counter == 1;
        }

        public bool IncrementSoftDespawnCount()
        {
            return --_counter == 0;
        }
    }
}
