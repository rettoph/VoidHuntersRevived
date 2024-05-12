using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities
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
        public readonly IReadOnlyDictionary<Type, IEntityComponent> StaticComponents;

        Id<IEntityType> IEntityType.Id => this.Id;
        string IEntityType.Key => this.Key;
        EntityTypeFlags IEntityType.Flags => this.Flags;
        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        IEntityType? IEntityType.BaseType => this.BaseType;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.InstanceComponents => this.InstanceComponents;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.StaticComponents => this.StaticComponents;

        internal unsafe EntityType(string key, EntityTypeFlags flags, VoidHuntersEntityDescriptor descriptor, IEntityType? baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            this.Key = key;
            this.Id = Id<IEntityType>.FromString(key);
            this.Flags = flags;

            _list.Add(this);
            this.Descriptor = descriptor;
            this.BaseType = baseType;

            this.InstanceComponents = instanceComponents.ToDictionary(x => x.GetType(), x => x);
            this.StaticComponents = staticComponents.ToDictionary(x => x.GetType(), x => x);
        }

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, Type descriptorType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            ThrowIf.Type.IsNotAssignableFrom<VoidHuntersEntityDescriptor>(descriptorType);

            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptorType);

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, instanceComponents, staticComponents)!;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, VoidHuntersEntityDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, descriptor, instanceComponents, staticComponents)!;
        }

        public static IEntityType Create(string key, EntityTypeFlags flags, IEntityType baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(baseType.Descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, flags, baseType, instanceComponents, staticComponents)!;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(string key, EntityTypeFlags flags, IEntityType baseType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents) : base(key, flags, baseType.Descriptor, baseType, baseType.InstanceComponents.Values.Concat(instanceComponents), baseType.StaticComponents.Values.Concat(staticComponents))
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags, TDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents) : base(key, flags, descriptor, null, instanceComponents, staticComponents)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents) : this(key, flags, new TDescriptor(), instanceComponents, staticComponents)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, EntityTypeFlags flags = EntityTypeFlags.None) : this(key, flags, Enumerable.Empty<IEntityComponent>(), Enumerable.Empty<IEntityComponent>())
        {
            this.Descriptor = new TDescriptor();
        }
    }
}
