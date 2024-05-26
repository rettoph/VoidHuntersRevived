using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct EntityStatus : IEntityComponent
    {
        private fixed int _modificationsCount[2];

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
