﻿using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor, IEntityResource<VoidHuntersEntityDescriptor>, IEquatable<VoidHuntersEntityDescriptor?>
    {
        private DynamicEntityDescriptor<StaticEntityDescriptor> _staticDescriptor;
        private DynamicEntityDescriptor<InstanceEntityDescriptor> _instanceDescriptor;
        private readonly List<ComponentManager> _componentManagers;
        private Id<VoidHuntersEntityDescriptor>? _id;
        private string? _name;

        public Id<VoidHuntersEntityDescriptor> Id => _id ??= HashBuilder<VoidHuntersEntityDescriptor, VhId>.Instance.CalculateId(VhId.HashString(this.GetType().AssemblyQualifiedName ?? throw new NotImplementedException()));
        public string Name => _name ??= this.GetType().Name;
        public Resource<Color> PrimaryColor { get; } = Resources.Colors.None;
        public Resource<Color> SecondaryColor { get; } = Resources.Colors.None;
        public int Order { get; } = 0;

        public IComponentBuilder[] componentsToBuild => _instanceDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

        public IEntityDescriptor StaticDescriptor => _staticDescriptor;

        protected VoidHuntersEntityDescriptor() : this(Resources.Colors.None, Resources.Colors.None, 0)
        {

        }
        protected unsafe VoidHuntersEntityDescriptor(Resource<Color> primaryColor, Resource<Color> secondaryColor, int order)
        {
            _staticDescriptor = DynamicEntityDescriptor<StaticEntityDescriptor>.CreateDynamicEntityDescriptor();
            _instanceDescriptor = DynamicEntityDescriptor<InstanceEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();

            this.PrimaryColor = primaryColor;
            this.SecondaryColor = secondaryColor;
            this.Order = order;
        }

        protected VoidHuntersEntityDescriptor WithInstanceComponents(ComponentManager[] managers)
        {
            var builders = managers.Select(x => x.Builder).ToArray();
            _instanceDescriptor.ExtendWith(builders);

            foreach (ComponentManager manager in managers)
            {
                _componentManagers.Add(manager);
            }

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
                if (this.ComponentManagers.Any(x => x.Type == componentType) == false)
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
