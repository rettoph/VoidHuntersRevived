using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class EntityInitializerHelper
    {
        public static EntityInitializerDelegate BuildEntityInitializerDelegate(IEntityComponent component)
        {
            var method = BuildEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            EntityInitializerDelegate initializer = (EntityInitializerDelegate)method.Invoke(null, new object[] { component })!;
            return initializer;
        }

        public static EntityInitializerDelegate? BuildEntityInitializerDelegate(IEnumerable<IEntityComponent> components)
        {
            EntityInitializerDelegate? initializer = default;

            foreach (IEntityComponent component in components)
            {
                initializer += BuildEntityInitializerDelegate(component);
            }

            return initializer;
        }

        public static MethodInfo BuildEntityInitializerDelegateMethodInfo = typeof(EntityInitializerHelper).GetMethod(nameof(BuildEntityInitializerDelegate), 1, new Type[] { Type.MakeGenericMethodParameter(0) }) ?? throw new Exception();
        public static EntityInitializerDelegate BuildEntityInitializerDelegate<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            return (IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(instance);
            };
        }
    }
}
