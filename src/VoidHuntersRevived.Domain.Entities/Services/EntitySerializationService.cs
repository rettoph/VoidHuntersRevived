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
using VoidHuntersRevived.Common.Entities.Descriptors;
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
        private readonly IEntityIdService _entities;
        private readonly IEntityDescriptorService _descriptors;
        private readonly IEngineService _engines;
        private readonly EntityTypeService _types;
        private readonly ILogger _logger;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntitySerializationService(
            IEventPublishingService events,
            IEntityIdService entities,
            IEntityDescriptorService descriptors,
            IEngineService engines,
            ILogger logger,
            EntityTypeService types)
        {
            _events = events;
            _entities = entities;
            _descriptors = descriptors;
            _engines = engines;
            _types = types;
            _logger = logger;
            _reader = new EntityReader(this);
            _writer = new EntityWriter(this);
        }

        public void Ready()
        {
        }

        public EntityData Serialize(EntityId id)
        {
            _writer.Reset();
            this.Serialize(id, _writer);
            return _writer.Export(id.VhId);
        }

        public void Serialize(EntityId id, EntityWriter writer)
        {
            VoidHuntersEntityDescriptor descriptor = _entities.GetEntityDescriptor(id.VhId);

            writer.Write(id.VhId);
            writer.WriteStruct(descriptor.Id);

            this.entitiesDB.QueryEntitiesAndIndex<EntityVhId>(id.EGID, out uint index);
            descriptor.Serialize(writer, id.EGID, this.entitiesDB, index);
        }

        public EntityId Deserialize(in VhId seed, EntityData data, bool confirmed)
        {
            _reader.Load(seed, data, confirmed);
            return this.Deserialize(_reader);
        }

        public EntityId Deserialize(EntityReader reader)
        {
            VhId vhid = reader.ReadVhId();
            VhId descriptorId = reader.ReadStruct<VhId>();

            VoidHuntersEntityDescriptor descriptor = _descriptors.GetById(descriptorId);
            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MathodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntitySerializationService), nameof(Deserialize), vhid.Value, descriptor.Name, reader.Seed.Value);

            SpawnEntityDescriptor createEntityEvent = new SpawnEntityDescriptor()
            {
                Descriptor = descriptor,
                VhId = vhid,
                Initializer = (ref EntityInitializer initializer) =>
                {
                    reader.Load(readerState);
                    descriptor.Deserialize(reader, ref initializer);

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

            return _entities.GetId(vhid);
        }

        public EntityData Serialize(VhId vhid)
        {
            return this.Serialize(_entities.GetId(vhid));
        }

        public EntityData Serialize(EGID egid)
        {
            return this.Serialize(_entities.GetId(egid));
        }

        public EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.Serialize(_entities.GetId(entityId, groupId));
        }

        public void Serialize(VhId vhid, EntityWriter writer)
        {
            this.Serialize(_entities.GetId(vhid), writer);
        }

        public void Serialize(EGID egid, EntityWriter writer)
        {
            this.Serialize(_entities.GetId(egid), writer);
        }

        public void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer)
        {
            this.Serialize(_entities.GetId(entityId, groupId), writer);
        }
    }
}
