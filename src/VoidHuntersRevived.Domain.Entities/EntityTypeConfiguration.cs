using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using VoidHuntersRevived.Domain.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class EntityTypeConfiguration : IEntityTypeConfiguration
    {
        private bool _initialized;
        private readonly HashSet<EntityType> _baseTypes;
        private readonly HashSet<Type> _components;
        private readonly HashSet<Type> _properties;
        private readonly List<IComponentBuilder> _builders;

        internal IEntityDescriptor descriptor;
        internal ExclusiveGroup group;

        public EntityType Type { get; }

        public EntityTypeConfiguration(EntityType type)
        {
            _baseTypes = new HashSet<EntityType>();
            _components = new HashSet<Type>();
            _properties = new HashSet<Type>();
            _builders = new List<IComponentBuilder>();

            this.descriptor = null!;
            this.group = null!;

            this.Type = type;

            this.HasComponent<EntityVhId>();
        }

        internal void Initialize(
            EntityTypeService types,
            EntityPropertyService properties)
        {
            if(_initialized)
            {
                return;
            }

            List<IComponentBuilder> builders = new List<IComponentBuilder>();
            this.GetComponentBuildersRecursive(new HashSet<EntityTypeConfiguration>(), builders, types, properties);
            this.descriptor = new DynamicEntityDescriptor<EntityDescriptor>(builders.ToArray());
            this.group = new ExclusiveGroup();
        }

        public IEntityTypeConfiguration Inherits(EntityType baseType)
        {
            _baseTypes.Add(baseType);

            return this;
        }

        public IEntityTypeConfiguration HasComponent<T>()
            where T : unmanaged, IEntityComponent
        {
            if(_components.Add(typeof(T)))
            {
                _builders.Add(new ComponentBuilder<T>());
            }

            return this;
        }

        private static readonly MethodInfo _genericHasMethodInfo = typeof(EntityTypeConfiguration)
            .GetMethod(nameof(HasComponent), 1, Array.Empty<Type>()) ?? throw new Exception();

        public IEntityTypeConfiguration HasComponent(Type component)
        {
            var method = _genericHasMethodInfo.MakeGenericMethod(component);
            method.Invoke(this, Array.Empty<object>());

            return this;
        }

        public IEntityTypeConfiguration HasProperty<T>() 
            where T : IEntityProperty
        {
            _properties.Add(typeof(T));

            return this;
        }

        private void GetComponentBuildersRecursive(
            HashSet<EntityTypeConfiguration> configurations,
            List<IComponentBuilder> components,
            EntityTypeService types,
            EntityPropertyService properties)
        {
            if (!configurations.Add(this))
            {
                return;
            }

            foreach(EntityType baseType in  _baseTypes)
            {
                types.GetOrCreateConfiguration(baseType).GetComponentBuildersRecursive(configurations, components, types, properties);
            }

            foreach (IComponentBuilder builder in _builders)
            {
                components.Add(builder);
            }

            foreach(Type propertyType in _properties)
            {
                foreach(IComponentBuilder builder in properties.GetOrCreateConfiguration(propertyType).builders)
                {
                    components.Add(builder);
                }
            }
        }
    }
}
