namespace VoidHuntersRevived.Common.Entities
{
    // public static class IEntityTypeConfigurationExtensions
    // {
    //     private static MethodInfo _initializeStaticComponentMethodInfo = typeof(IEntityTypeConfiguration).GetMethod(nameof(InitializeStaticComponent), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
    //     public static IEntityTypeConfiguration InitializeStaticComponent(this IEntityTypeConfiguration configuration, IEntityComponent component)
    //     {
    //         var method = _initializeStaticComponentMethodInfo.MakeGenericMethod(component.GetType());
    // 
    //         method.Invoke(configuration, new object[] { component });
    //         return configuration;
    //     }
    // 
    //     private static MethodInfo _initializeInstanceComponentMethodInfo = typeof(IEntityTypeConfigurationExtensions).GetMethod(nameof(InitializeInstanceComponent), 1, new Type[] { typeof(IEntityTypeConfiguration), Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
    //     public static IEntityTypeConfiguration InitializeInstanceComponent(this IEntityTypeConfiguration configuration, IEntityComponent component)
    //     {
    //         var method = _initializeInstanceComponentMethodInfo.MakeGenericMethod(component.GetType());
    // 
    //         method.Invoke(null, new object[] { configuration, component });
    //         return configuration;
    //     }
    // 
    //     public static IEntityTypeConfiguration InitializeInstanceComponent<T>(this IEntityTypeConfiguration configuration, T instance)
    //         where T : unmanaged, IEntityComponent
    //     {
    //         return configuration.InitializeInstanceComponent<T>(new EntityTypeComponentInitializer<T>()
    //         {
    //             Instance = instance,
    //             Dispose = () =>
    //             {
    //                 if (instance is not IDisposable disposable)
    //                 {
    //                     return;
    //                 }
    // 
    //                 AutoDisposeComponentAttribute? autoDisposeAttr = instance.GetType()
    //                     .GetCustomAttributes(true)
    //                     .OfType<AutoDisposeComponentAttribute>()
    //                     .FirstOrDefault(x => x.GetDisposableComponentType(typeof(T)) == typeof(T));
    //                 if ((autoDisposeAttr?.Scope ?? AutoDisposeScope.None) == AutoDisposeScope.Type)
    //                 {
    //                     disposable.Dispose();
    //                 }
    //             }
    //         });
    //     }
    // 
    //     public static IEntityTypeConfiguration InitializeInstanceComponent<T>(this IEntityTypeConfiguration configuration, T instance, Action dispose)
    //         where T : unmanaged, IEntityComponent
    //     {
    //         return configuration.InitializeInstanceComponent<T>(new EntityTypeComponentInitializer<T>()
    //         {
    //             Instance = instance,
    //             Dispose = dispose
    //         });
    //     }
    // 
    //     public static IEntityTypeConfiguration InitializeInstanceComponent<T>(this IEntityTypeConfiguration configuration, Func<EntityId, T> instance)
    //         where T : unmanaged, IEntityComponent
    //     {
    //         return configuration.InitializeInstanceComponent<T>(new EntityTypeComponentFactoryInitializer<T>()
    //         {
    //             Instance = instance,
    //             Dispose = null
    //         });
    //     }
    // 
    //     public static IEntityTypeConfiguration InitializeInstanceComponent<T>(this IEntityTypeConfiguration configuration, Func<EntityId, T> instance, Action dispose)
    //         where T : unmanaged, IEntityComponent
    //     {
    //         return configuration.InitializeInstanceComponent<T>(new EntityTypeComponentFactoryInitializer<T>()
    //         {
    //             Instance = instance,
    //             Dispose = dispose
    //         });
    //     }
    // 
    //     internal sealed class EntityTypeComponentFactoryInitializer<T> : IEntityTypeComponentInitializer<T>
    //         where T : unmanaged, IEntityComponent
    //     {
    //         public Action? Dispose { get; init; }
    //         public Func<EntityId, T> Instance { get; init; } = null!;
    // 
    //         public T GetInstance(EntityId id)
    //         {
    //             return this.Instance(id);
    //         }
    // 
    //         void IDisposable.Dispose()
    //         {
    //             this.Dispose?.Invoke();
    //         }
    //     }
    // 
    //     internal sealed class EntityTypeComponentInitializer<T> : IEntityTypeComponentInitializer<T>
    //         where T : unmanaged, IEntityComponent
    //     {
    //         public Action? Dispose { get; init; }
    //         public T Instance { get; init; }
    // 
    //         public T GetInstance(EntityId id)
    //         {
    //             return this.Instance;
    //         }
    // 
    //         void IDisposable.Dispose()
    //         {
    //             this.Dispose?.Invoke();
    //         }
    //     }
    // }
}
