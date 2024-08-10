using Guppy.Core.Common.Utilities;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct InstanceEntity : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;

        public IEntityType Type => _typeRef.Value;

        public InstanceEntity(UnmanagedReference<IEntityType> typeRef)
        {
            _typeRef = typeRef;
        }
    }
}
