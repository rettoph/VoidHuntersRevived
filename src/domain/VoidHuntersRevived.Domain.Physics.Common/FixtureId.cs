using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public readonly struct FixtureId(uint index, EntityLocalId entityLocalId)
    {
        public readonly uint Index = index;
        public readonly EntityLocalId EntityLocalId = entityLocalId;

        public override bool Equals(object? obj)
        {
            return obj is FixtureId id &&
                   Index == id.Index &&
                   EqualityComparer<EntityLocalId>.Default.Equals(EntityLocalId, id.EntityLocalId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, EntityLocalId);
        }

        public static bool operator ==(FixtureId left, FixtureId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FixtureId left, FixtureId right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{this.Index}:{this.EntityLocalId}";
        }
    }
}
