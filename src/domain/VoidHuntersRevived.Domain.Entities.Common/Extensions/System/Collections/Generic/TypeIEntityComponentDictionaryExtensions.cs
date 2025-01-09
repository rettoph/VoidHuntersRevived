using Guppy.Core.Common;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions.System.Collections.Generic
{
    public static class TypeIEntityComponentDictionaryExtensions
    {
        public static IComponentBuilder[] ToComponentBuilders(this Dictionary<Type, IEntityComponent> components)
        {
            List<IComponentBuilder> builders = [];

            foreach ((Type type, object value) in components)
            {
                ThrowIf.Type.IsNotUnmanagedStruct(type);
                ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(type);
                ThrowIf.Type.IsNotAssignableFrom(type, value.GetType());

                Type componentBuilderType = typeof(ComponentBuilder<>).MakeGenericType(type);
                IComponentBuilder builder = (IComponentBuilder)(Activator.CreateInstance(componentBuilderType, value) ?? throw new NotImplementedException());

                builders.Add(builder);
            }

            return [.. builders];
        }
    }
}