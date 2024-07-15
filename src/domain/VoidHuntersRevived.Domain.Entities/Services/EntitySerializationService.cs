using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EntitySerializationService : StrategyEngine, IEntitySerializationService
    {
        private readonly EntityReader _reader;
        private readonly EntityWriter _writer;

        public EntitySerializationService(
            EntityWriter writer,
            EntityReader reader)
        {
            _reader = reader;
            _writer = writer;
        }

        public EntityData Serialize(EntityId id, SerializationOptions options)
        {
            return _writer.Serialize(id, options);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer);
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            return _reader.Deserialize(sourceId, data, options, initializer, rootInitializer);
        }
    }
}
