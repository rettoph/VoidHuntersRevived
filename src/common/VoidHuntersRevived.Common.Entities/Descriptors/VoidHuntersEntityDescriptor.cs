using Svelto.ECS;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor, IEquatable<VoidHuntersEntityDescriptor?>
    {
        private DynamicEntityDescriptor<StaticEntityDescriptor> _staticDescriptor;
        private DynamicEntityDescriptor<InstanceEntityDescriptor> _instanceDescriptor;
        private Id<VoidHuntersEntityDescriptor>? _id;
        private string? _name;

        public Id<VoidHuntersEntityDescriptor> Id => _id ??= HashBuilder<VoidHuntersEntityDescriptor, VhId>.Instance.CalculateId(VhId.HashString(this.GetType().AssemblyQualifiedName ?? throw new NotImplementedException()));
        public string Name => _name ??= this.GetType().Name;

        public IComponentBuilder[] componentsToBuild => _instanceDescriptor.componentsToBuild;

        public IEntityDescriptor StaticDescriptor => _staticDescriptor;

        protected VoidHuntersEntityDescriptor()
        {
            _staticDescriptor = DynamicEntityDescriptor<StaticEntityDescriptor>.CreateDynamicEntityDescriptor();
            _instanceDescriptor = DynamicEntityDescriptor<InstanceEntityDescriptor>.CreateDynamicEntityDescriptor();
        }

        protected VoidHuntersEntityDescriptor WithInstanceComponents(IComponentBuilder[] builders)
        {
            _instanceDescriptor.ExtendWith(builders);

            return this;
        }

        protected VoidHuntersEntityDescriptor WithStaticComponents(IComponentBuilder[] builders)
        {
            _staticDescriptor.ExtendWith(builders);

            return this;
        }

        public bool HasAll(params Type[] componentTypes)
        {
            foreach (Type componentType in componentTypes)
            {
                if (this._instanceDescriptor.componentsToBuild.Any(x => x.GetEntityComponentType() == componentType) == false)
                {
                    return false;
                }
            }

            return true;
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
