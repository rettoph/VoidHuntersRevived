using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common
{
    public readonly struct VhId : IEquatable<VhId>
    {
        public static readonly VhId Empty = default!;

        public readonly Guid Value;

        public VhId(string g)
        {
            this.Value = new Guid(g);
        }
        public VhId(Guid guid)
        {
            this.Value = guid;
        }
        public static VhId NewId()
        {
            return new(Guid.NewGuid());
        }

        public override bool Equals(object? obj)
        {
            return obj is VhId id && this.Equals(id);
        }

        public readonly bool Equals(VhId other)
        {
            return this.Value == other.Value;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Value);
        }

        public override readonly string ToString()
        {
            return this.Value.ToString();
        }

        public static bool operator ==(VhId left, VhId right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(VhId left, VhId right)
        {
            return left.Value != right.Value;
        }

        public static unsafe VhId HashString(string value)
        {
            uint128 nameHash = xxHash128.ComputeHash(value);
            VhId* pNameHash = (VhId*)&nameHash;

            return pNameHash[0];
        }
    }
}