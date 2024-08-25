using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal class HasManyEntityInitializer : BaseEntityTypeProviderInitializer
    {
        private readonly Dictionary<IEntityTypeProvider, EntityInitializerDelegate> _instanceInitializers;
        private readonly Dictionary<IEntityTypeProvider, EntityInitializerDelegate> _typeInitializers;

        public HasManyEntityInitializer()
        {
            _instanceInitializers = new Dictionary<IEntityTypeProvider, EntityInitializerDelegate>();
            _typeInitializers = new Dictionary<IEntityTypeProvider, EntityInitializerDelegate>();

            this.WithEntityInitializer(provider => provider.Components.Keys.Any(componentType =>
            {
                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<,>))
                {
                    return false;
                }

                // Build initializer now
                if (_instanceInitializers.ContainsKey(provider) == false)
                {
                    _instanceInitializers.Add(provider, HasManyEntityInitializersBuilder(provider.Components.Keys));
                }

                return true;

            }), this.InitializeInstanceParentComponents);
        }

        private void InitializeInstanceParentComponents(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer)
        {
            _instanceInitializers[provider](entities, provider, id, ref initializer);
        }

        private void InitializeTypeParentComponents(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer)
        {
            _typeInitializers[provider](entities, provider, id, ref initializer);
        }

        private static MethodInfo hasManyComponentInitializerBuilderMethod = typeof(HasManyEntityInitializer).GetMethod(nameof(HasManyEntityInitializer.HasManyComponentInitializerBuilder), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new NotImplementedException();
        private static EntityInitializerDelegate HasManyEntityInitializersBuilder(IEnumerable<Type> componentTypes)
        {

            EntityInitializerDelegate initializers = default!;

            foreach (Type componentType in componentTypes)
            {
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
            return (IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(new HasMany<TItem, T>(id, entities.GetReference()));
            };
        }
    }
}
