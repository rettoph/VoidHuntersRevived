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
        private readonly Stack<EntityLocalId> _nested = [];
        private bool _serializing;

        public EntityData Serialize(EntityLocalId localId, SerializationOptions options)
        {
            if (_entityQueryService.TryGetEntity(localId, out Entity entity) == false)
            {
                throw new NotImplementedException();
            }

            return this.Serialize(ref entity, options);
        }

        public EntityData Serialize(ExclusiveGroupStruct groupId, uint index, SerializationOptions options)
        {
            if (_entityQueryService.TryGetEntity(groupId, index, out Entity entity) == false)
            {
                throw new NotImplementedException();
            }

            return this.Serialize(ref entity, options);
        }

        public EntityLocalId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer)
        {
            _logger.Verbose("Starting Deserialization. Id = {Id}, OwnerId = {OwnerId}, Seed = {Seed}", data.Id, options.Owner, options.Seed);

            EntityLocalId entityLocalId = this.InternalDeserialize(sourceId, data, 0, options, initializer);
            for (int i = 1; i < data.IndexCount; i++)
            {
                this.InternalDeserialize(sourceId, data, i, options, initializer);
            }

            return entityLocalId;
        }

        public EntityLocalId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            EntityLocalId entityLocalId = this.InternalDeserialize(sourceId, data, 0, options, rootInitializer + initializer);
            for (int i = 1; i < data.IndexCount; i++)
            {
                this.InternalDeserialize(sourceId, data, i, options, initializer);
            }

            return entityLocalId;
        }

        private EntityData Serialize(ref Entity entity, SerializationOptions options)
        {
            if (_serializing == true)
            {
                throw new NotImplementedException();
            }

            _logger.Verbose("Starting Serialization. GlobalId = {GlobalId}, LocalId = {LocalId}", entity.GlobalId, entity.LocalId);


            try
            {
                _serializing = true;

                EntityGlobalId globalId = _entityQueryService.QueryByLocalId<EntityGlobalId>(entity.LocalId, out GroupIndex groupIndex);
                this.InternalSerialize(ref entity, options);

                while (_nested.TryPop(out EntityLocalId nestedLocalId))
                {
                    EntityGlobalId nestedGlobalId = _entityQueryService.QueryByLocalId<EntityGlobalId>(nestedLocalId, out GroupIndex nestedGroupIndex);
                    Entity nestedEntity = new(nestedGroupIndex.Index, nestedLocalId, nestedGlobalId);
                    this.InternalSerialize(ref nestedEntity, options);
                }

                EntityData result = new(globalId.Value, _data.ToArray(), _indices.ToArray());

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

        private void InternalSerialize(ref Entity entity, SerializationOptions options)
        {
            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<EntityTemplateComponent>(entity.GroupIndex).Key;

            EntityWriter writer = new(_data, _nested);

            _indices.Add(_data.Count);
            writer.Write(entity.GlobalId);
            writer.Write(templateKey.Id);

            _logger.Verbose("Preparing to serialize. GlobalId = {GlobalId}, LocalId = {LocaLId}, Template = {EntityTemplate}", entity.GlobalId, entity.LocalId, templateKey);

            _entityTemplateService.GetByKey(templateKey).SerializeInstanceEntity(ref writer, in entity, in options);
        }

        public static readonly unsafe int EntityHeaderSize = sizeof(VhId) + sizeof(Id<EntityTemplateFragment>);

        private EntityLocalId InternalDeserialize(
            VhId sourceId,
            EntityData data,
            int index,
            DeserializationOptions options,
            EntityInitializerDelegate initializerDelegate)
        {
            EntityReader reader = data.GetReader(options.Seed, index);

            EntityGlobalId entityGlobalId = reader.ReadGlobalEntityId();
            Key<IEntityTemplate> entityTemplateKey = Key<IEntityTemplate>.GetById(reader.Read<VhId>());

            _logger.Verbose("Preparing to deserialize. GlobalId = {GlobalId}, Template = {Template}, Seed = {Seed}, Index = {Index}, DataId = {DataId}", entityGlobalId.Value, entityTemplateKey.Name, options.Seed.Value, index, data.Id);

            return _entitySpawnService.Spawn(sourceId, entityTemplateKey, entityGlobalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                EntityReader reader = data.GetReader(options.Seed, index, EntityHeaderSize);

                entity.Template.DeserializeInstanceEntity(in sourceId, in options, ref reader, in entity);

                initializerDelegate(entities, in entity);
            });
        }
    }
}
