using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct EntityType(UnmanagedReference<IEntityType> typeRef) : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef = typeRef;

        public IEntityType Value => _typeRef.Value;
        public readonly Key<IEntityType> Key = typeRef.Value.Key;
    }
}
