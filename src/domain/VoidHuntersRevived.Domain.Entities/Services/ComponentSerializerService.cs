using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class ComponentSerializerService(Lazy<IFiltered<IComponentSerializer>> serializers) : IComponentSerializerService
    {
        private readonly Lazy<IFiltered<IComponentSerializer>> _serializers = serializers;
        private readonly Dictionary<Type, IComponentSerializer> _serializersTable = [];

        public void Initialize()
        {
            foreach (IComponentSerializer serializer in this._serializers.Value)
            {
                this._serializersTable.Add(serializer.Type, serializer);
            }
        }

        public IComponentSerializer GetComponentSerializerByType(Type componentType)
        {
            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(componentType);
            ThrowIf.Type.IsNotUnmanagedStruct(componentType);

            return this._serializersTable[componentType];
        }

        public IEnumerable<IComponentSerializer> GetComponentSerializersByTypes(IEnumerable<Type> componentTypes)
        {
            foreach (Type componentType in componentTypes)
            {
                if (this._serializersTable.TryGetValue(componentType, out IComponentSerializer? componentSerializer))
                {
                    yield return componentSerializer;
                }
            }
        }
    }
}