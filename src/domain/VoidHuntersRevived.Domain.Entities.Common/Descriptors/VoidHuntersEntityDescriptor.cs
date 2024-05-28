using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IEquatable<VoidHuntersEntityDescriptor?>
    {
        private DynamicEntityDescriptor<TypeEntityDescriptor> _typeDescriptor;
        private DynamicEntityDescriptor<InstanceEntityDescriptor> _instanceDescriptor;
        private Id<VoidHuntersEntityDescriptor>? _id;

        public Id<VoidHuntersEntityDescriptor> Id => _id ??= HashBuilder<VoidHuntersEntityDescriptor, VhId>.Instance.CalculateId(VhId.HashString(this.GetType().AssemblyQualifiedName ?? throw new NotImplementedException()));
        public string Name { get; }

        public IEntityDescriptor Instance => _instanceDescriptor;
        public IEntityDescriptor Type => _typeDescriptor;

        public ExclusiveGroupStruct InstanceGroup { get; }
        public ExclusiveGroupStruct TypeGroup { get; }

        protected VoidHuntersEntityDescriptor()
        {
            _typeDescriptor = DynamicEntityDescriptor<TypeEntityDescriptor>.CreateDynamicEntityDescriptor();
            _instanceDescriptor = DynamicEntityDescriptor<InstanceEntityDescriptor>.CreateDynamicEntityDescriptor();

            this.Name = this.GetType().Name;
            this.InstanceGroup = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{this.Name}_{nameof(this.InstanceGroup)}");
            this.TypeGroup = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{this.Name}}}_{nameof(this.TypeGroup)}");
        }

        protected VoidHuntersEntityDescriptor WithInstanceComponents(IComponentBuilder[] builders)
        {
            _instanceDescriptor.ExtendWith(builders);

            return this;
        }

        protected VoidHuntersEntityDescriptor WithTypeComponents(IComponentBuilder[] builders)
        {
            _typeDescriptor.ExtendWith(builders);

            return this;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as VoidHuntersEntityDescriptor);
        }

        public bool Equals(VoidHuntersEntityDescriptor? other)
        {
            return other is not null &&
                   this.GetType() == other.GetType();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.GetType());
        }

        public static bool operator ==(VoidHuntersEntityDescriptor? left, VoidHuntersEntityDescriptor? right)
        {
            return EqualityComparer<VoidHuntersEntityDescriptor>.Default.Equals(left, right);
        }

        public static bool operator !=(VoidHuntersEntityDescriptor? left, VoidHuntersEntityDescriptor? right)
        {
            return !(left == right);
        }
    }
}
