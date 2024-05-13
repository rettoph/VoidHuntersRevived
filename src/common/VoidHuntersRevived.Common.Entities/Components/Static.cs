using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct Static<T> : IEntityComponent
        where T : VoidHuntersEntityDescriptor
    {
        private readonly StaticValue<VoidHuntersEntityDescriptor, T> _value;

        public T Value => _value;

        public Static(T value)
        {
            _value = new StaticValue<VoidHuntersEntityDescriptor, T>(value);
        }
    }
}
