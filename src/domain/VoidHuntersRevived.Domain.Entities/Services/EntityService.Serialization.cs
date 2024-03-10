using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        public EntityData Serialize(EntityId id, SerializationOptions options)
        {
            return _writer.Serialize(id, options);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, InstanceEntityInitializerDelegate? initializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer);
        }
    }
}
