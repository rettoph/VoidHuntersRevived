using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
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
