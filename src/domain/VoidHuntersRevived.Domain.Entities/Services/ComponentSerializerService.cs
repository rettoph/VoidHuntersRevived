using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class ComponentSerializerService(IFiltered<ComponentSerializer> serializers) : IComponentSerializerService
    {
        private Dictionary<Type, ComponentSerializer> _serializers = serializers.ToDictionary(x => x.Type, x => x);

        public ComponentSerializer GetComponentSerializerByType(Type componentType)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(componentType);
            ThrowIf.Type.IsNotUnmanagedStruct(componentType);

            return _serializers[componentType];
        }

        public IEnumerable<ComponentSerializer> GetComponentSerializersByTypes(IEnumerable<Type> componentTypes)
        {
            foreach (Type componentType in componentTypes)
            {
                if (_serializers.TryGetValue(componentType, out ComponentSerializer? componentSerializer))
                {
                    yield return componentSerializer;
                }
            }
        }
    }
}
