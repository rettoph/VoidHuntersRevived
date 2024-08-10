using Guppy.Core.Common.Utilities;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct TypeEntity : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;

        public IEntityType Type => _typeRef.Value;

        public TypeEntity(UnmanagedReference<IEntityType> typeRef)
        {
            _typeRef = typeRef;
        }
    }
}
