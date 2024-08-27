using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct EntityType : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;

        public IEntityType Value => _typeRef.Value;
        public readonly Key<IEntityType> Key;

        public EntityType(UnmanagedReference<IEntityType> typeRef)
        {
            _typeRef = typeRef;

            this.Key = typeRef.Value.Key;
        }
    }
}
