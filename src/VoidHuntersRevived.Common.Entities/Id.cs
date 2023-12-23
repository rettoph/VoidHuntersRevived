using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities
{
    public struct Id<T> : IEntityComponent, IEquatable<Id<T>>
    {
        private readonly VhId _value;

        public VhId Value => _value;

        public Id(VhId value)
        {
            _value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Id<T> id && Equals(id);
        }

        public bool Equals(Id<T> other)
        {
            return _value.Value == other._value.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
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
            return new Id<T>(VhId.HashString(input));
        }

        public override string ToString()
        {
            return typeof(T).Name + ":" + _value.ToString();
        }
    }
}
