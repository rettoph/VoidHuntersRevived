using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public abstract class EntityType : IEntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly Id<IEntityType> Id;
        public readonly string Key;
        public readonly EntityTypeFlags Flags;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly IEntityType? BaseType;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> InstanceComponents;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> Components;

        Id<IEntityType> IEntityType.Id => this.Id;
        string IEntityType.Key => this.Key;
        EntityTypeFlags IEntityType.Flags => this.Flags;
        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        IEntityType? IEntityType.BaseType => this.BaseType;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.InstanceComponents => this.InstanceComponents;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.Components => this.Components;

        internal unsafe EntityType(string key, EntityTypeFlags flags, VoidHuntersEntityDescriptor descriptor, IEntityType? baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components)
        {
            this.Key = key;
            this.Id = Id<IEntityType>.FromString(key);
            this.Flags = flags;

            _list.Add(this);
            this.Descriptor = descriptor;
            this.BaseType = baseType;

            this.InstanceComponents = instanceComponents.ToDictionary(x => x.GetType(), x => x);
            this.Components = components.ToDictionary(x => x.GetType(), x => x);
        }

        public override string ToString()
        {
            return $"{this.Key}:{this.Descriptor.Name}";
        }

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, Type descriptorType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components)
        {
            ThrowIf.Type.IsNotAssignableFrom<VoidHuntersEntityDescriptor>(descriptorType);

            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptorType);

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, instanceComponents, components)!;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, VoidHuntersEntityDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, descriptor, instanceComponents, components)!;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, IEntityType baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(baseType.Descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, baseType, instanceComponents, components)!;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(string key, EntityTypeFlags flags, IEntityType baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components) : base(key, flags, baseType.Descriptor, baseType, baseType.InstanceComponents.Values.Concat(instanceComponents), baseType.Components.Values.Concat(components))
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags, TDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components) : base(key, flags, descriptor, null, instanceComponents, components)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> components) : this(key, flags, new TDescriptor(), instanceComponents, components)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags = EntityTypeFlags.None) : this(key, flags, Enumerable.Empty<IEntityComponent>(), Enumerable.Empty<IEntityComponent>())
        {
            this.Descriptor = new TDescriptor();
        }
    }
}
