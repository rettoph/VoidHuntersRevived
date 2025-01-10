using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityGlobalId(VhId vhid) : IEntityComponent, IEquatable<EntityGlobalId>
    {
        public readonly VhId Value = vhid;

        public override bool Equals(object? obj) => obj is EntityGlobalId id &&
                   this.Value.Value == id.Value.Value;

        public bool Equals(EntityGlobalId other) => this.Value.Value == other.Value.Value;

        public override int GetHashCode() => HashCode.Combine(this.Value);

        public static bool operator ==(EntityGlobalId left, EntityGlobalId right)
        {
            return left.Value.Value == right.Value.Value;
        }

        public static bool operator !=(EntityGlobalId left, EntityGlobalId right)
        {
            return left.Value.Value != right.Value.Value;
        }

        public override string ToString() => this.Value.ToString();
    }
}