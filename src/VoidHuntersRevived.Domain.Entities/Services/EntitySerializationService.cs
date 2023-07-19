using Guppy.Attributes;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntitySerializationService : IEntitySerializationService, IQueryingEntitiesEngine
    {
        private readonly EntityReader _reader;
        private readonly EntityWriter _writer;
        private readonly IEventPublishingService _events;
        private readonly IEntityService _entities;
        private readonly IEngineService _engines;
        private readonly EntityTypeService _types;
        private readonly ILogger _logger;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntitySerializationService(
            IEventPublishingService events,
            IEntityService entities,
            IEngineService engines,
            ILogger logger,
            EntityTypeService types)
        {
            _events = events;
            _entities = entities;
            _engines = engines;
            _types = types;
            _logger = logger;
            _reader = new EntityReader(this);
            _writer = new EntityWriter(this);
        }

        public void Ready()
        {
        }

        public EntityData Serialize(IdMap id)
        {
            _writer.Reset();
            this.Serialize(id, _writer);
            return _writer.Export(id.VhId);
        }

        public void Serialize(IdMap id, EntityWriter writer)
        {
            IEntityType type = _entities.GetEntityType(id.VhId);

            writer.Write(id.VhId);
            writer.WriteStruct(type.Id);

            this.entitiesDB.QueryEntitiesAndIndex<EntityVhId>(id.EGID, out uint index);
            type.Descriptor.Serialize(writer, id.EGID, this.entitiesDB, index);
        }

        public IdMap Deserialize(in VhId seed, EntityData data, bool confirmed)
        {
            _reader.Load(seed, data, confirmed);
            return this.Deserialize(_reader);
        }

        public IdMap Deserialize(EntityReader reader)
        {
            VhId vhid = reader.ReadVhId();
            VhId typeId = reader.ReadStruct<VhId>();

            IEntityType type = _types.GetById(typeId);
            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MathodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntitySerializationService), nameof(Deserialize), vhid.Value, type.Name, reader.Seed.Value);

            CreateEntity createEntityEvent = new CreateEntity()
            {
                Type = type,
                VhId = vhid,
                Configure = false,
                Initializer = (ref EntityInitializer initializer) =>
                {
                    reader.Load(readerState);
                    type.Descriptor.Deserialize(reader, ref initializer);

                    reader.Busy = false;
                }
            };

            if(reader.Confirmed)
            {
                _events.Confirm(vhid, createEntityEvent);
            }
            else
            {
                _events.Publish(vhid, createEntityEvent);
            }

            return _entities.GetIdMap(vhid);
        }

        public EntityData Serialize(VhId vhid)
        {
            return this.Serialize(_entities.GetIdMap(vhid));
        }

        public EntityData Serialize(EGID egid)
        {
            return this.Serialize(_entities.GetIdMap(egid));
        }

        public EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.Serialize(_entities.GetIdMap(entityId, groupId));
        }

        public void Serialize(VhId vhid, EntityWriter writer)
        {
            this.Serialize(_entities.GetIdMap(vhid), writer);
        }

        public void Serialize(EGID egid, EntityWriter writer)
        {
            this.Serialize(_entities.GetIdMap(egid), writer);
        }

        public void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer)
        {
            this.Serialize(_entities.GetIdMap(entityId, groupId), writer);
        }
    }
}
