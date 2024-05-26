using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Entities.Descriptors
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

        public ExclusiveGroupStruct Group { get; }

        protected VoidHuntersEntityDescriptor()
        {
            _typeDescriptor = DynamicEntityDescriptor<TypeEntityDescriptor>.CreateDynamicEntityDescriptor();
            _instanceDescriptor = DynamicEntityDescriptor<InstanceEntityDescriptor>.CreateDynamicEntityDescriptor();

            this.Name = this.GetType().Name;
            this.Group = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct(this.Name);

            VoidHuntersEntityDescriptor.GetDescriptorComponentBuilders(this, out IComponentBuilder[] instanceDescriptorComponents, out IComponentBuilder[] staticComponentBuilders);
            this.WithInstanceComponents(instanceDescriptorComponents);
            this.WithTypeComponents(staticComponentBuilders);
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

        private static Dictionary<Type, IComponentBuilder[][]> _descriptorComponentBuilders = new Dictionary<Type, IComponentBuilder[][]>();
        private static void GetDescriptorComponentBuilders(VoidHuntersEntityDescriptor descriptor, out IComponentBuilder[] instanceComponentBuilders, out IComponentBuilder[] staticComponentBuilders)
        {
            Type? type = descriptor.GetType();
            ref IComponentBuilder[][]? builders = ref CollectionsMarshal.GetValueRefOrAddDefault(_descriptorComponentBuilders, type, out bool exists);
            if (exists == true)
            {
                instanceComponentBuilders = builders![0];
                staticComponentBuilders = builders[1];
            }

            List<IComponentBuilder> instanceBuilderList = new List<IComponentBuilder>();
            List<IComponentBuilder> staticBuilderList = new List<IComponentBuilder>();
            while (type is not null && type != typeof(object))
            {
                IComponentBuilder instanceDescriptorComponentBuilder = VoidHuntersEntityDescriptor.MakeDescriptorComponent(typeof(Instance<>), type, descriptor);
                IComponentBuilder staticDescriptorComponentBuilder = VoidHuntersEntityDescriptor.MakeDescriptorComponent(typeof(Type<>), type, descriptor);

                instanceBuilderList.Add(instanceDescriptorComponentBuilder);
                staticBuilderList.Add(staticDescriptorComponentBuilder);

                type = type.BaseType;
            }

            instanceComponentBuilders = instanceBuilderList.ToArray();
            staticComponentBuilders = staticBuilderList.ToArray();
            builders = [instanceComponentBuilders, staticComponentBuilders];
        }

        private static IComponentBuilder MakeDescriptorComponent(Type genericComponentDefinition, Type descriptorType, VoidHuntersEntityDescriptor descriptor)
        {
            Type descriptorComponentType = genericComponentDefinition.MakeGenericType(descriptorType);
            Type descriptorComponentBuilderType = typeof(ComponentBuilder<>).MakeGenericType(descriptorComponentType);

            object descriptorComponent = Activator.CreateInstance(descriptorComponentType, descriptor) ?? throw new InvalidOperationException();
            IComponentBuilder descriptorComponentBuilder = Activator.CreateInstance(descriptorComponentBuilderType, descriptorComponent) as IComponentBuilder ?? throw new InvalidOperationException();

            return descriptorComponentBuilder;
        }
    }
}
