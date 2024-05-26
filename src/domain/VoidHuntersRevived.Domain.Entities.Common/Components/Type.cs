using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct Type<T> : IEntityComponent
        where T : VoidHuntersEntityDescriptor
    {
        private readonly StaticValue<VoidHuntersEntityDescriptor, T> _value;

        public T Value => _value;

        public Type(T value)
        {
            _value = new StaticValue<VoidHuntersEntityDescriptor, T>(value);
        }
    }
}
