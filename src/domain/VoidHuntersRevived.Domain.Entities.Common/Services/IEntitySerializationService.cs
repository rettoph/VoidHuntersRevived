using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntitySerializationService
    {
        Serialization.EntityData Serialize(EntityLocalId localId, SerializationOptions options);

        EntityId Deserialize(VhId sourceId, DeserializationOptions options, Serialization.EntityData data, EntityInitializerDelegate initializer);
        EntityId Deserialize(VhId sourceId, DeserializationOptions options, Serialization.EntityData data, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer);
    }
}
