using System.Reflection;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class EntityInitializerHelper
    {
        public static EntityInitializerDelegate BuildEntityInitializerDelegate(IEntityComponent component)
        {
            var method = _buildEntityInitializerDelegateMethodInfo.MakeGenericMethod(component.GetType());

            EntityInitializerDelegate initializer = (EntityInitializerDelegate)method.Invoke(null, [component])!;
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

        private static readonly MethodInfo _buildEntityInitializerDelegateMethodInfo = typeof(EntityInitializerHelper).GetMethod(nameof(BuildEntityInitializerDelegate), 1, [Type.MakeGenericMethodParameter(0)]) ?? throw new Exception();
        public static EntityInitializerDelegate BuildEntityInitializerDelegate<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            return (IEntityService entities, in InitializingEntity entity) =>
                                                              {
                                                                  entity.Initializer.Init(instance);
                                                              };
        }
    }
}