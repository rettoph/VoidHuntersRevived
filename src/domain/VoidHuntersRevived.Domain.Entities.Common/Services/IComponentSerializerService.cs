using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IComponentSerializerService
    {
        IComponentSerializer GetComponentSerializerByType(Type componentType);

        IEnumerable<IComponentSerializer> GetComponentSerializersByTypes(IEnumerable<Type> componentTypes);

        FasterList<IComponentSerializer> GetComponentSerializersByDescriptor(IEntityDescriptor descriptor)
        {
            IEnumerable<Type> componentTypes = descriptor.componentsToBuild.Select(x => x.GetEntityComponentType());
            IComponentSerializer[] instanceEntityComponentSerializers = this.GetComponentSerializersByTypes(componentTypes).ToArray();
            return new FasterList<IComponentSerializer>(instanceEntityComponentSerializers);
        }
    }
}
