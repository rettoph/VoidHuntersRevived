using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        public EntityData Serialize(EntityId id, SerializationOptions options)
        {
            return _writer.Serialize(id, options);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, InstanceEntityInitializerDelegate initializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, InstanceEntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer, rootInitializer);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, InstanceEntityInitializerDelegate initializer, InstanceEntityInitializerDelegate rootInitializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer, rootInitializer);
        }
    }
}
