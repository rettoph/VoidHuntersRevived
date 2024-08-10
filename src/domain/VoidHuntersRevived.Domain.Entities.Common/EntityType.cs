using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    [Service<IEntityType>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public class EntityType : IEntityType
    {
        public HashSet<Type> RequiredInstanceEntityComponents { get; }
        public ComponentBuilderDictionary InstanceEntityComponentBuilders { get; }

        public HashSet<Type> RequiredTypeEntityComponents { get; }
        public ComponentBuilderDictionary TypeEntityComponentBuilders { get; }

        public IKey<IEntityType> Key { get; }
        public EntityTypeFlags Flags { get; set; }
        public IKey<IEntityType>[] Include { get; }
        public Type Type => this.GetType();

        public EntityType(IKey<IEntityType> key, IKey<IEntityType>[] include)
        {
            this.RequiredInstanceEntityComponents = new HashSet<Type>();
            this.RequiredTypeEntityComponents = new HashSet<Type>();

            this.InstanceEntityComponentBuilders = new ComponentBuilderDictionary();
            this.TypeEntityComponentBuilders = new ComponentBuilderDictionary();

            this.Key = key;
            this.Include = include;

            foreach (IKey<IEntityType> includeTypeKey in include)
            {
                ThrowIf.Type.IsNotAssignableFrom(includeTypeKey.Type, this.Type);
            }

            // TODO: Some of these components should just be marked as required rather than
            // Given default values.
            this.WithInstanceEntityComponents([
                new EntityId(),
                new EntityStatus(),
                new InstanceEntity(),
                new BelongsTo<TypeEntity, InstanceEntity>()
            ]);

            this.WithTypeEntityComponents([
                new EntityId(),
                new TypeEntity(),
                new HasMany<InstanceEntity, TypeEntity>()
            ]);
        }

        public EntityType WithFlags(EntityTypeFlags flags)
        {
            this.Flags |= flags;

            return this;
        }

        public EntityType WithInstanceEntityComponent(IEntityComponent component)
        {
            this.InstanceEntityComponentBuilders.Set(component);

            return this;
        }

        public EntityType WithInstanceEntityComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent
        {
            this.InstanceEntityComponentBuilders.Set(component);

            return this;
        }

        public EntityType WithInstanceEntityComponents(IEnumerable<IEntityComponent> components)
        {
            foreach (IEntityComponent component in components)
            {
                this.WithInstanceEntityComponent(component);
            }

            return this;
        }

        public EntityType RequireInstanceEntityComponent(Type component)
        {
            return this.RequireEntityComponent(this.RequiredInstanceEntityComponents, component);
        }

        public EntityType RequireInstanceEntityComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            return this.RequireEntityComponent(this.RequiredInstanceEntityComponents, typeof(TComponent));
        }

        public EntityType RequireInstanceEntityComponents(Type[] components)
        {
            foreach (Type component in components)
            {
                this.RequireEntityComponent(this.RequiredInstanceEntityComponents, component);
            }

            return this;
        }

        public EntityType RequireTypeEntityComponent(Type component)
        {
            return this.RequireEntityComponent(this.RequiredTypeEntityComponents, component);
        }

        public EntityType RequireTypeEntityComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            return this.RequireEntityComponent(this.RequiredTypeEntityComponents, typeof(TComponent));
        }

        public EntityType RequireTypeEntityComponents(Type[] components)
        {
            foreach (Type component in components)
            {
                return this.RequireEntityComponent(this.RequiredTypeEntityComponents, component);
            }

            return this;
        }

        public EntityType WithTypeEntityComponent(IEntityComponent component)
        {
            this.TypeEntityComponentBuilders.Set(component);

            return this;
        }

        public EntityType WithTypeEntityComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent
        {
            this.TypeEntityComponentBuilders.Set(component);

            return this;
        }

        public EntityType WithTypeEntityComponents(IEnumerable<IEntityComponent> components)
        {
            foreach (IEntityComponent component in components)
            {
                this.WithTypeEntityComponent(component);
            }

            return this;
        }

        private EntityType RequireEntityComponent(HashSet<Type> components, Type type)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(type);
            ThrowIf.Type.IsNotUnmanagedStruct(type);

            components.Add(type);

            return this;
        }
    }
}
