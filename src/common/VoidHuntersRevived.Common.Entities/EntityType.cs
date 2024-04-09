using Guppy.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class EntityType : IEntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly Id<IEntityType> Id;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly string Key;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> InstanceComponents;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> StaticComponents;

        Id<IEntityType> IEntityType.Id => this.Id;
        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        string IEntityType.Key => this.Key;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.InstanceComponents => this.InstanceComponents;
        IReadOnlyDictionary<Type, IEntityComponent> IEntityType.StaticComponents => this.StaticComponents;

        internal unsafe EntityType(string key, VoidHuntersEntityDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            this.Key = key;
            this.Id = Id<IEntityType>.FromString(key);

            _list.Add(this);
            this.Descriptor = descriptor;

            this.InstanceComponents = instanceComponents.ToDictionary(x => x.GetType(), x => x);
            this.StaticComponents = staticComponents.ToDictionary(x => x.GetType(), x => x);
        }

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }

        public static IEntityType Create(string key, Type descriptorType, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            ThrowIf.Type.IsNotAssignableFrom<VoidHuntersEntityDescriptor>(descriptorType);

            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptorType);

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, instanceComponents, staticComponents)!;
        }

        public static IEntityType Create(string key, VoidHuntersEntityDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key, descriptor, instanceComponents, staticComponents)!;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(string key, TDescriptor descriptor, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents) : base(key, descriptor, instanceComponents, staticComponents)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key, IEnumerable<IEntityComponent> instanceComponents, IEnumerable<IEntityComponent> staticComponents) : this(key, new TDescriptor(), instanceComponents, staticComponents)
        {
            this.Descriptor = new TDescriptor();
        }
        public EntityType(string key) : this(key, Enumerable.Empty<IEntityComponent>(), Enumerable.Empty<IEntityComponent>())
        {
            this.Descriptor = new TDescriptor();
        }
    }
}
