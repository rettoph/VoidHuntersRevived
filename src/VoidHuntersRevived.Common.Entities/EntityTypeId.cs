using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct EntityTypeId : IEntityComponent, IEquatable<EntityTypeId>
    {
        private readonly VhId _value;

        public VhId Value => _value;

        public EntityTypeId(VhId value)
        {
            _value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityTypeId id && Equals(id);
        }

        public bool Equals(EntityTypeId other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static bool operator ==(EntityTypeId left, EntityTypeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityTypeId left, EntityTypeId right)
        {
            return !(left == right);
        }
    }
}
