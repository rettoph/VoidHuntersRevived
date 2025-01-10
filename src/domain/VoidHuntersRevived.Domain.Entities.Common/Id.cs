using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct Id<T>(VhId value) : IId<T>, IEquatable<Id<T>>
    {
        public readonly VhId Value { get; } = value;

        public override bool Equals(object? obj)
        {
            return obj is Id<T> id && this.Equals(id);
        }

        public readonly bool Equals(Id<T> other)
        {
            return this.Value.Value == other.Value.Value;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Value);
        }

        public static bool operator ==(Id<T> left, Id<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id<T> left, Id<T> right)
        {
            return !(left == right);
        }

        public static Id<T> FromString(string input)
        {
            return new(NameSpace<T>.Instance.Create(input));
        }

        public override readonly string ToString()
        {
            return typeof(T).Name + ":" + this.Value.ToString();
        }
    }
}