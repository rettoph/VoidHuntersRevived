using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common
{
    public struct VhId : IEquatable<VhId>
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
            return new VhId(Guid.NewGuid());
        }

        public override bool Equals(object? obj)
        {
            return obj is VhId id && Equals(id);
        }

        public bool Equals(VhId other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString()
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
