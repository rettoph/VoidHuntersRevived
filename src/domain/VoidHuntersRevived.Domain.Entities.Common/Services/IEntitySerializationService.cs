using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySerializationService
    {
        EntityData Serialize(EntityLocalId localId, SerializationOptions options);
        EntityData Serialize(ExclusiveGroupStruct groupId, uint index, SerializationOptions options);

        EntityLocalId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer);
        EntityLocalId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer);
    }
}
