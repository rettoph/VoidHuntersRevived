using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct Instance<T> : IEntityComponent
        where T : VoidHuntersEntityDescriptor
    {
        private readonly UnmanagedReference<VoidHuntersEntityDescriptor, T> _value;

        public T Value => _value;

        public Instance(T value)
        {
            _value = new UnmanagedReference<VoidHuntersEntityDescriptor, T>(value);
        }
    }
}
