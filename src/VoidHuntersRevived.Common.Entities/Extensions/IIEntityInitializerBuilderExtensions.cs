using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class IIEntityInitializerBuilderExtensions
    {
        public static IEntityInitializerBuilder InitializeInstanceComponent(this IEntityInitializerBuilder builder, IEntityComponent component)
        {
            var method = BuildInstanceEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            InstanceEntityInitializerDelegate initializer = (InstanceEntityInitializerDelegate)method.Invoke(builder, new object[] { component })!;
            return builder.InitializeInstance(initializer);
        }

        public static IEntityInitializerBuilder InitializeStaticComponent(this IEntityInitializerBuilder builder, IEntityComponent component)
        {
            var method = BuildStaticEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            StaticEntityInitializerDelegate initializer = (StaticEntityInitializerDelegate)method.Invoke(builder, new object[] { component })!;
            return builder.InitializeStatic(initializer);
        }

        public static MethodInfo BuildInstanceEntityInitializerDelegateMethodInfo = typeof(IIEntityInitializerBuilderExtensions).GetMethod(nameof(BuildInstanceEntityInitializerDelegate), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
        public static InstanceEntityInitializerDelegate BuildInstanceEntityInitializerDelegate<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            return (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(instance);
            };
        }

        public static MethodInfo BuildStaticEntityInitializerDelegateMethodInfo = typeof(IIEntityInitializerBuilderExtensions).GetMethod(nameof(BuildStaticEntityInitializerDelegate), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
        public static StaticEntityInitializerDelegate BuildStaticEntityInitializerDelegate<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            return (ref EntityInitializer initializer) =>
            {
                initializer.Init(instance);
            };
        }
    }
}
