using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public readonly struct FixtureId(uint index, EntityLocalId entityLocalId)
    {
        public readonly uint Index = index;
        public readonly EntityLocalId EntityLocalId = entityLocalId;

        public override bool Equals(object? obj) => obj is FixtureId id &&
                   this.Index == id.Index &&
                   EqualityComparer<EntityLocalId>.Default.Equals(this.EntityLocalId, id.EntityLocalId);

        public override int GetHashCode() => HashCode.Combine(this.Index, this.EntityLocalId);

        public static bool operator ==(FixtureId left, FixtureId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FixtureId left, FixtureId right)
        {
            return !(left == right);
        }

        public override string ToString() => $"{this.Index}:{this.EntityLocalId}";
    }
}