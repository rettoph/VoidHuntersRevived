using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IComponentSerializerService
    {
        ComponentSerializer GetComponentSerializer(Type componentType);

        IEnumerable<ComponentSerializer> GetComponentSerializers(IEnumerable<Type> componentTypes);
    }
}
