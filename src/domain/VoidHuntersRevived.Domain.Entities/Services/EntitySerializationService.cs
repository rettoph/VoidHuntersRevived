using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using EntityTemplateComponent = VoidHuntersRevived.Domain.Entities.Common.Components.EntityTemplate;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntitySerializationService(
        IEntityTemplateService entityTemplateService,
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        ILogger logger) : StrategyEngine, IEntitySerializationService
    {
        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;

        // Serialization buffers
        private readonly List<byte> _data = [];
        private readonly List<int> _indices = [];
        private readonly Stack<EntityId> _nested = [];
        private bool _serializing;

        public EntityData Serialize(EntityId id, SerializationOptions options)
        {
            if (_serializing == true)
            {
                throw new NotImplementedException();
            }

            try
            {
                _serializing = true;

                this.InternalSerialize(id, options);
                while (_nested.TryPop(out EntityId nestedId))
                {
                    this.InternalSerialize(nestedId, options);
                }

                EntityData result = new(id.VhId, _data.ToArray(), _indices.ToArray());

                return result;
            }
            finally
            {
                // Reset serialization buffers
                _data.Clear();
                _indices.Clear();
                _nested.Clear();

                _serializing = false;
            }
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer)
        {
            EntityId entityId = this.InternalDeserialize(sourceId, data, 0, options, initializer);
            for (int i = 1; i < data.IndexCount; i++)
            {
                this.InternalDeserialize(sourceId, data, i, options, initializer);
            }

            return entityId;
        }

        public EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            EntityId entityId = this.InternalDeserialize(sourceId, data, 0, options, rootInitializer + initializer);
            for (int i = 1; i < data.IndexCount; i++)
            {
                this.InternalDeserialize(sourceId, data, i, options, initializer);
            }

            return entityId;
        }

        private void InternalSerialize(EntityId id, SerializationOptions options)
        {
            Key<IEntityTemplate> templateKey = _entityQueryService.QueryById<EntityTemplateComponent>(id, out GroupIndex groupIndex).Key;

            EntityWriter writer = new(_data, _nested);

            _indices.Add(_data.Count);
            writer.Write(id.VhId);
            writer.Write(templateKey.Id);

            _logger.Verbose("Preparing to serialize {EntityId} of type {EntityTemplate}", id.VhId, templateKey);

            _entityTemplateService.GetByKey(templateKey).SerializeInstanceEntity(ref writer, in id, in groupIndex, in options);
        }

        public static readonly unsafe int EntityHeaderSize = sizeof(VhId) + sizeof(Id<EntityTemplateFragment>);

        private EntityId InternalDeserialize(
            VhId sourceId,
            EntityData data,
            int index,
            DeserializationOptions options,
            EntityInitializerDelegate initializerDelegate)
        {
            EntityReader reader = data.GetReader(options.Seed, index);

            VhId entityVhId = reader.ReadVhId();
            Key<IEntityTemplate> entityTemplateKey = Key<IEntityTemplate>.GetById(reader.Read<VhId>());

            _logger.Verbose("Preparing to deserialize {EntityId} of type {EntityTemplate} with seed {seed}", entityVhId.Value, entityTemplateKey, options.Seed.Value);

            return _entitySpawnService.Spawn(sourceId, entityTemplateKey, entityVhId, (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                EntityReader reader = data.GetReader(options.Seed, index, EntityHeaderSize);

                _entityTemplateService.GetByKey(entityTemplateKey).DeserializeInstanceEntity(in sourceId, in options, ref reader, ref initializer, in id);

                initializerDelegate(entities, entityTemplate, id, ref initializer);
            });
        }
    }
}
