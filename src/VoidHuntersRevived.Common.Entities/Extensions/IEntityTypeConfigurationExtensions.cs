using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Attributes;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities
{
    public static class IEntityTypeConfigurationExtensions
    {
        private static MethodInfo _initializeComponentMethodInfo = typeof(IEntityTypeConfigurationExtensions).GetMethod(nameof(InitializeComponent), 1, new Type[] { typeof(IEntityTypeConfiguration), Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
        public static IEntityTypeConfiguration InitializeComponent(this IEntityTypeConfiguration configuration, IEntityComponent component)
        {
            var method = _initializeComponentMethodInfo.MakeGenericMethod(component.GetType());

            method.Invoke(null, new object[] { configuration, component });
            return configuration;
        }

        public static IEntityTypeConfiguration InitializeComponent<T>(this IEntityTypeConfiguration configuration, T instance)
            where T : unmanaged, IEntityComponent
        {
            return configuration.InitializeComponent<T>(new EntityTypeComponentInstanceInitializer<T>()
            {
                Instance = instance,
                Dispose = () =>
                {
                    if(instance is not IDisposable disposable)
                    {
                        return;
                    }

                    AutoDisposeComponentAttribute? autoDisposeAttr = instance.GetType()
                        .GetCustomAttributes(true)
                        .OfType<AutoDisposeComponentAttribute>()
                        .FirstOrDefault(x => x.GetDisposableComponentType(typeof(T)) == typeof(T));
                    if((autoDisposeAttr?.Scope ?? AutoDisposeScope.None) == AutoDisposeScope.Type)
                    {
                        disposable.Dispose();
                    }
                }
            });
        }

        public static IEntityTypeConfiguration InitializeComponent<T>(this IEntityTypeConfiguration configuration, T instance, Action dispose)
            where T : unmanaged, IEntityComponent
        {
            return configuration.InitializeComponent<T>(new EntityTypeComponentInstanceInitializer<T>()
            {
                Instance = instance,
                Dispose = dispose
            });
        }

        public static IEntityTypeConfiguration InitializeComponent<T>(this IEntityTypeConfiguration configuration, Func<EntityId, T> instance)
            where T : unmanaged, IEntityComponent
        {
            return configuration.InitializeComponent<T>(new EntityTypeComponentFactoryInitializer<T>()
            {
                Instance = instance,
                Dispose = null
            });
        }

        public static IEntityTypeConfiguration InitializeComponent<T>(this IEntityTypeConfiguration configuration, Func<EntityId, T> instance, Action dispose)
            where T : unmanaged, IEntityComponent
        {
            return configuration.InitializeComponent<T>(new EntityTypeComponentFactoryInitializer<T>()
            {
                Instance = instance,
                Dispose = dispose
            });
        }

        internal sealed class EntityTypeComponentFactoryInitializer<T> : IEntityTypeComponentValue<T>
            where T : unmanaged, IEntityComponent
        {
            public Action? Dispose { get; init; }
            public Func<EntityId, T> Instance { get; init; } = null!;

            public T GetInstance(EntityId id)
            {
                return this.Instance(id);
            }

            void IDisposable.Dispose()
            {
                this.Dispose?.Invoke();
            }
        }

        internal sealed class EntityTypeComponentInstanceInitializer<T> : IEntityTypeComponentValue<T>
            where T : unmanaged, IEntityComponent
        {
            public Action? Dispose { get; init; }
            public T Instance { get; init; }

            public T GetInstance(EntityId id)
            {
                return this.Instance;
            }

            void IDisposable.Dispose()
            {
                this.Dispose?.Invoke();
            }
        }
    }
}
