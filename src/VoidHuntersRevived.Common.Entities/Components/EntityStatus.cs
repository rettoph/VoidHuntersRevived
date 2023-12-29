using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public unsafe struct EntityStatus : IEntityComponent
    {
        private fixed int _modificationsCount[2];
#if DEBUG
        private int _modificationsCount0 => (int)_modificationsCount[0];
        private int _modificationsCount1 => (int)_modificationsCount[1];
        private int _spawnedCount => this.GetSpawnedCount();
#endif

        public EntityStatusEnum Value;

        public EntityStatus(EntityStatusEnum value)
        {
            this.Value = value;
            this.Increment(EntityModificationTypeEnum.Spawned);
        }

        public bool IsSpawned => this.Value == EntityStatusEnum.SoftSpawned;
        public bool IsDespawned => this.Value > EntityStatusEnum.SoftSpawned;

        public int Increment(EntityModificationTypeEnum modification)
        {
            ref int count = ref this.InternalGetModificationCount(modification);
            count = count + 1;

            return _modificationsCount[0] - _modificationsCount[1];
        }

        public int GetSpawnedCount()
        {
            return _modificationsCount[0] - _modificationsCount[1];
        }

        private unsafe ref int InternalGetModificationCount(EntityModificationTypeEnum modification)
        {
            int index = (int)modification;
            return ref _modificationsCount[index];
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(EntityStatus lhs, EntityStatus rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator !=(EntityStatus lhs, EntityStatus rhs)
        {
            return lhs.Value != rhs.Value;
        }
    }
}
