using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal class HasManyEntityInitializer : BaseEntityInitializer
    {
        private readonly Dictionary<IEntityType, EntityInitializerDelegate> _instanceInitializers;
        private readonly Dictionary<IEntityType, EntityInitializerDelegate> _typeInitializers;

        public HasManyEntityInitializer(IEntityService entities)
        {
            _instanceInitializers = new Dictionary<IEntityType, EntityInitializerDelegate>();
            _typeInitializers = new Dictionary<IEntityType, EntityInitializerDelegate>();

            this.WithInstanceInitializer(type => type.Descriptor.Instance.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<,>))
                {
                    return false;
                }

                // Build initializer now
                if (_instanceInitializers.ContainsKey(type) == false)
                {
                    _instanceInitializers.Add(type, HasManyEntityInitializersBuilder(type.Descriptor.Instance.componentsToBuild));
                }

                return true;

            }), this.InitializeInstanceParentComponents);

            this.WithTypeInitializer(type => type.Descriptor.Type.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<,>))
                {
                    return false;
                }

                // Build initializer now
                if (_typeInitializers.ContainsKey(type) == false)
                {
                    _typeInitializers.Add(type, HasManyEntityInitializersBuilder(type.Descriptor.Type.componentsToBuild));
                }

                return true;

            }), this.InitializeTypeParentComponents);
        }

        private void InitializeInstanceParentComponents(IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer)
        {
            _instanceInitializers[type](entities, type, id, ref initializer);
        }

        private void InitializeTypeParentComponents(IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer)
        {
            _typeInitializers[type](entities, type, id, ref initializer);
        }

        private static MethodInfo hasManyComponentInitializerBuilderMethod = typeof(HasManyEntityInitializer).GetMethod(nameof(HasManyEntityInitializer.HasManyComponentInitializerBuilder), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new NotImplementedException();
        private static EntityInitializerDelegate HasManyEntityInitializersBuilder(IComponentBuilder[] components)
        {

            EntityInitializerDelegate initializers = default!;

            foreach (IComponentBuilder component in components)
            {
                Type componentType = component.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    continue;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<,>))
                {
                    continue;
                }

                initializers += (EntityInitializerDelegate)hasManyComponentInitializerBuilderMethod.MakeGenericMethod(componentType.GenericTypeArguments).Invoke(null, Array.Empty<object>())!;
            }

            return initializers;
        }

        private static EntityInitializerDelegate HasManyComponentInitializerBuilder<TItem, T>()
            where TItem : unmanaged, IEntityComponent
            where T : unmanaged, IEntityComponent
        {
            return (IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(new HasMany<TItem, T>(id, entities.GetReference()));
            };
        }
    }
}
