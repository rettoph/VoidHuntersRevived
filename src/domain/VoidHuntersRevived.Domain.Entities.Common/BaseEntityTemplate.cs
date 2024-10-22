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
    [Service<IEntityTemplate>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public class BaseEntityTemplate : IEntityTemplate
    {
        private readonly UnmanagedReference<IEntityTemplate> _ref;

        public HashSet<Type> RequiredComponents { get; }
        public ComponentBuilderDictionary Components { get; }

        public Key<IEntityTemplate> Key { get; }
        public Type Type => this.GetType();

        public EntityInitializerDelegate? Initializer { get; set; }

        public BaseEntityTemplate(Key<IEntityTemplate> key)
        {
            ThrowIf.Type.IsNotAssignableFrom(key.Type, this.Type);

            _ref = new UnmanagedReference<IEntityTemplate>(this);

            this.Key = key;
            this.RequiredComponents = [];
            this.Components = new ComponentBuilderDictionary();

            // TODO: Some of these components should just be marked as required rather than
            // Given default values.
            this.WithComponents([
                new EntityId(),
                new EntityStatus(),
                new EntityTemplate(_ref)
            ]);
        }

        public IEntityTemplate WithComponent(IEntityComponent component)
        {
            this.Components.Set(component);

            return this;
        }

        public IEntityTemplate WithComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent
        {
            this.Components.Set(component);

            return this;
        }

        public IEntityTemplate WithComponents(IEnumerable<IEntityComponent> components)
        {
            foreach (IEntityComponent component in components)
            {
                this.WithComponent(component);
            }

            return this;
        }

        public IEntityTemplate RequireComponent(Type component)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(component);
            ThrowIf.Type.IsNotUnmanagedStruct(component);

            this.RequiredComponents.Add(component);

            return this;
        }

        public IEntityTemplate RequireComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            this.RequiredComponents.Add(typeof(TComponent));

            return this;
        }

        public IEntityTemplate RequireComponents(IEnumerable<Type> components)
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
                throw new EntityTemplateException(this.Key, $"EntityTemplate '{this.Key.Name}' => Missing required components: '{string.Join(',', missingRequiredComponents)}'");
            }
        }
    }
}
