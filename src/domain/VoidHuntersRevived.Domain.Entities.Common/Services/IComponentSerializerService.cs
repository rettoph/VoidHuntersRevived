using Svelto.DataStructures;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IComponentSerializerService
    {
        FasterList<ComponentSerializer> GetInstanceComponentSerializers(IEntityType type);
    }
}
