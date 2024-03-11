using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IComponentSerializerService
    {
        FasterList<ComponentSerializer> GetComponentSerializers(IEntityDescriptor descriptor);
    }
}
