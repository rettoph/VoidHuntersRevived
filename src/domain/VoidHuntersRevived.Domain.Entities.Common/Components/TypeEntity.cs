using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct TypeEntity : IEntityComponent
    {
        private readonly UnmanagedReference<IEntityType> _typeRef;

        public IEntityType Type => _typeRef.Value;

        public readonly Id<IEntityType> TypeId;
        public readonly Id<VoidHuntersEntityDescriptor> DescriptorId;

        public TypeEntity(UnmanagedReference<IEntityType> typeRef)
        {
            _typeRef = typeRef;

            this.TypeId = typeRef.Value.Id;
            this.DescriptorId = typeRef.Value.Descriptor.Id;
        }
    }
}
