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
            if (this._entityQueryService.TryGetEntity(localId, out Entity entity) == false)
            {
                throw new NotImplementedException();
            }

            return this.Serialize(ref entity, options);
        }

        public EntityData Serialize(ExclusiveGroupStruct groupId, uint index, SerializationOptions options)
        {
            if (this._entityQueryService.TryGetEntity(groupId, index, out Entity entity) == false)
            {
                throw new NotImplementedException();
            }

            return this.Serialize(ref entity, options);
        }

        public EntityLocalId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer)
        {
            this._logger.Verbose("Starting Deserialization. Id = {Id}, OwnerId = {OwnerId}, Seed = {Seed}", data.Id, options.Owner, options.Seed);

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
            if (this._serializing == true)
            {
                throw new NotImplementedException();
            }

            this._logger.Verbose("Starting Serialization. GlobalId = {GlobalId}, LocalId = {LocalId}", entity.GlobalId, entity.LocalId);


            try
            {
                this._serializing = true;

                EntityGlobalId globalId = this._entityQueryService.QueryByLocalId<EntityGlobalId>(entity.LocalId, out GroupIndex groupIndex);
                this.InternalSerialize(ref entity, options);

                while (this._nested.TryPop(out EntityLocalId nestedLocalId))
                {
                    EntityGlobalId nestedGlobalId = this._entityQueryService.QueryByLocalId<EntityGlobalId>(nestedLocalId, out GroupIndex nestedGroupIndex);
                    Entity nestedEntity = new(nestedGroupIndex.Index, nestedLocalId, nestedGlobalId);
                    this.InternalSerialize(ref nestedEntity, options);
                }

                EntityData result = new(globalId.Value, [.. this._data], [.. this._indices]);

                return result;
            }
            finally
            {
                // Reset serialization buffers
                this._data.Clear();
                this._indices.Clear();
                this._nested.Clear();

                this._serializing = false;
            }
        }

        private void InternalSerialize(ref Entity entity, SerializationOptions options)
        {
            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<EntityTemplateComponent>(entity.GroupIndex).Key;

            EntityWriter writer = new(this._data, this._nested);

            this._indices.Add(this._data.Count);
            writer.Write(entity.GlobalId);
            writer.Write(templateKey.Id);

            this._logger.Verbose("Preparing to serialize. GlobalId = {GlobalId}, LocalId = {LocaLId}, Template = {EntityTemplate}", entity.GlobalId, entity.LocalId, templateKey);

            this._entityTemplateService.GetByKey(templateKey).SerializeInstanceEntity(ref writer, in entity, in options);
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

            this._logger.Verbose("Preparing to deserialize. GlobalId = {GlobalId}, Template = {Template}, Seed = {Seed}, Index = {Index}, DataId = {DataId}", entityGlobalId.Value, entityTemplateKey.Name, options.Seed.Value, index, data.Id);

            return this._entitySpawnService.Spawn(sourceId, entityTemplateKey, entityGlobalId, (IEntityService entities, in InitializingEntity entity) =>
            {
                EntityReader reader = data.GetReader(options.Seed, index, EntityHeaderSize);

                entity.Template.DeserializeInstanceEntity(in sourceId, in options, ref reader, in entity);

                initializerDelegate(entities, in entity);
            });
        }
    }
}