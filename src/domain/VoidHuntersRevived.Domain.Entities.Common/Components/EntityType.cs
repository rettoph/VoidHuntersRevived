using Guppy.Core.Common.Utilities;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct EntityType : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;

        public IEntityType Value => _typeRef.Value;

        public EntityType(UnmanagedReference<IEntityType> typeRef)
        {
            _typeRef = typeRef;
        }
    }
}
