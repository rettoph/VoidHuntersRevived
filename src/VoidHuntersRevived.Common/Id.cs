using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public struct Id<T>
    {
        private readonly VhId _value;

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
    }
}
