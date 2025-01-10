using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityLocalId(EGID egid) : IEntityComponent
    {
        public readonly EGID Value = egid;

        public ExclusiveGroupStruct Group => this.Value.groupID;

        public override bool Equals(object? obj)
        {
            return obj is EntityLocalId id &&
                   this.Value.Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public static bool operator ==(EntityLocalId left, EntityLocalId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityLocalId left, EntityLocalId right)
        {
            return !(left == right);
        }
    }
}