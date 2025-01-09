using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class ComponentSerializerService(IFiltered<IComponentSerializer> serializers) : IComponentSerializerService
    {
        private readonly Dictionary<Type, IComponentSerializer> _serializers = serializers.ToDictionary(x => x.Type, x => x);

        public IComponentSerializer GetComponentSerializerByType(Type componentType)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(componentType);
            ThrowIf.Type.IsNotUnmanagedStruct(componentType);

            return this._serializers[componentType];
        }

        public IEnumerable<IComponentSerializer> GetComponentSerializersByTypes(IEnumerable<Type> componentTypes)
        {
            foreach (Type componentType in componentTypes)
            {
                if (this._serializers.TryGetValue(componentType, out IComponentSerializer? componentSerializer))
                {
                    yield return componentSerializer;
                }
            }
        }
    }
}