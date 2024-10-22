using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public readonly struct EntityTemplate(UnmanagedReference<IEntityTemplate> typeRef) : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityTemplate> _typeRef = typeRef;

        public IEntityTemplate Value => _typeRef.Value;
        public readonly Key<IEntityTemplate> Key = typeRef.Value.Key;
    }
}
