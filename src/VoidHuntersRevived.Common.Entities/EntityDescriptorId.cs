using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct EntityDescriptorId : IEntityComponent, IEquatable<EntityDescriptorId>
    {
        private readonly VhId _value;

        public VhId Value => _value;

        public EntityDescriptorId(VhId value)
        {
            _value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityDescriptorId id && Equals(id);
        }

        public bool Equals(EntityDescriptorId other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static bool operator ==(EntityDescriptorId left, EntityDescriptorId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityDescriptorId left, EntityDescriptorId right)
        {
            return !(left == right);
        }
    }
}
