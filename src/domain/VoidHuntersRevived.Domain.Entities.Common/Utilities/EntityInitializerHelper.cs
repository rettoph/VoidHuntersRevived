using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Utilities
{
    public static class EntityInitializerHelper
    {
        public static InstanceEntityInitializerDelegate BuildInstanceEntityInitializerDelegate(IEntityComponent component)
        {
            var method = BuildInstanceEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            InstanceEntityInitializerDelegate initializer = (InstanceEntityInitializerDelegate)method.Invoke(null, new object[] { component })!;
            return initializer;
        }

        public static InstanceEntityInitializerDelegate? BuildInstanceEntityInitializerDelegate(IEnumerable<IEntityComponent> components)
        {
            InstanceEntityInitializerDelegate? initializer = default;

            foreach (IEntityComponent component in components)
            {
                initializer += BuildInstanceEntityInitializerDelegate(component);
            }

            return initializer;
        }

        public static StaticEntityInitializerDelegate BuildStaticEntityInitializerDelegate(IEntityComponent component)
        {
            var method = BuildStaticEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            StaticEntityInitializerDelegate initializer = (StaticEntityInitializerDelegate)method.Invoke(null, new object[] { component })!;
            return initializer;
        }

        public static StaticEntityInitializerDelegate? BuildStaticEntityInitializerDelegate(IEnumerable<IEntityComponent> components)
        {
            StaticEntityInitializerDelegate? initializer = default;

            foreach(IEntityComponent component in components)
            {
                initializer += BuildStaticEntityInitializerDelegate(component);
            }

            return initializer;
        }

        public static MethodInfo BuildInstanceEntityInitializerDelegateMethodInfo = typeof(EntityInitializerHelper).GetMethod(nameof(BuildInstanceEntityInitializerDelegate), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
        public static InstanceEntityInitializerDelegate BuildInstanceEntityInitializerDelegate<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            return (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(instance);
            };
        }

        public static MethodInfo BuildStaticEntityInitializerDelegateMethodInfo = typeof(EntityInitializerHelper).GetMethod(nameof(BuildStaticEntityInitializerDelegate), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
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
