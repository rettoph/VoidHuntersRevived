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
        public HashSet<Type> RequiredComponents { get; }
        public ComponentBuilderDictionary Components { get; }

        public IKey<IEntityType> Key { get; }
        public EntityTypeFlags Flags { get; set; }
        public IKey<IEntityType>[] Include { get; }
        public Type Type => this.GetType();

        public EntityType(IKey<IEntityType> key, IKey<IEntityType>[] include)
        {
            this.RequiredComponents = new HashSet<Type>();
            this.Components = new ComponentBuilderDictionary();

            this.Key = key;
            this.Include = include;

            foreach (IKey<IEntityType> includeTypeKey in include)
            {
                ThrowIf.Type.IsNotAssignableFrom(includeTypeKey.Type, this.Type);
            }

            // TODO: Some of these components should just be marked as required rather than
            // Given default values.
            this.WithComponents([
                new EntityId(),
                new EntityStatus(),
            ]);
        }

        public EntityType WithFlags(EntityTypeFlags flags)
        {
            this.Flags |= flags;

            return this;
        }

        public EntityType WithComponent(IEntityComponent component)
        {
            this.Components.Set(component);

            return this;
        }

        public EntityType WithComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent
        {
            this.Components.Set(component);

            return this;
        }

        public EntityType WithComponents(IEnumerable<IEntityComponent> components)
        {
            foreach (IEntityComponent component in components)
            {
                this.WithComponent(component);
            }

            return this;
        }

        public EntityType RequireComponent(Type component)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(component);
            ThrowIf.Type.IsNotUnmanagedStruct(component);

            this.RequiredComponents.Add(component);

            return this;
        }

        public EntityType RequireComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            this.RequiredComponents.Add(typeof(TComponent));

            return this;
        }

        public EntityType RequireComponents(Type[] components)
        {
            foreach (Type component in components)
            {
                this.RequireComponent(component);
            }

            return this;
        }
    }
}
