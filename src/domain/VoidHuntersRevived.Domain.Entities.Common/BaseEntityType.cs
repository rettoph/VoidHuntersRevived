using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    [Service<IEntityType>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public class BaseEntityType : IEntityType
    {
        private readonly UnmanagedReference<IEntityType> _ref;

        public HashSet<Type> RequiredComponents { get; }
        public ComponentBuilderDictionary Components { get; }

        public Key<IEntityType> Key { get; }
        public Type Type => this.GetType();

        public EntityInitializerDelegate? Initializer { get; set; }

        public BaseEntityType(Key<IEntityType> key)
        {
            ThrowIf.Type.IsNotAssignableFrom(key.Type, this.Type);

            _ref = new UnmanagedReference<IEntityType>(this);

            this.Key = key;
            this.RequiredComponents = [];
            this.Components = new ComponentBuilderDictionary();

            // TODO: Some of these components should just be marked as required rather than
            // Given default values.
            this.WithComponents([
                new EntityId(),
                new EntityStatus(),
                new EntityType(_ref)
            ]);
        }

        public IEntityType WithComponent(IEntityComponent component)
        {
            this.Components.Set(component);

            return this;
        }

        public IEntityType WithComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent
        {
            this.Components.Set(component);

            return this;
        }

        public IEntityType WithComponents(IEnumerable<IEntityComponent> components)
        {
            foreach (IEntityComponent component in components)
            {
                this.WithComponent(component);
            }

            return this;
        }

        public IEntityType RequireComponent(Type component)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(component);
            ThrowIf.Type.IsNotUnmanagedStruct(component);

            this.RequiredComponents.Add(component);

            return this;
        }

        public IEntityType RequireComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            this.RequiredComponents.Add(typeof(TComponent));

            return this;
        }

        public IEntityType RequireComponents(IEnumerable<Type> components)
        {
            foreach (Type component in components)
            {
                this.RequireComponent(component);
            }

            return this;
        }

        public void Verify()
        {
            List<string> missingRequiredComponents = [];

            foreach (Type requiredType in this.RequiredComponents)
            {
                if (this.Components.Keys.Contains(requiredType) == false)
                {
                    missingRequiredComponents.Add(requiredType.Name);
                }
            }

            if (missingRequiredComponents.Count > 0)
            {
                throw new EntityTypeException(this.Key, $"EntityType '{this.Key.Name}' => Missing required components: '{string.Join(',', missingRequiredComponents)}'");
            }
        }
    }
}
