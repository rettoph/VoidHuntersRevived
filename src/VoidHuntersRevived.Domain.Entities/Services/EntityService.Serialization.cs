﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Options;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly EntityReader _reader = new EntityReader();
        private readonly EntityWriter _writer = new EntityWriter();

        public EntityData Serialize(EntityId id, SerializationOptions options)
        {
            _writer.Reset();
            this.Serialize(id, _writer, options);
            return _writer.Export(id.VhId);
        }

        public void Serialize(EntityId id, EntityWriter writer, SerializationOptions options)
        {
            EntityTypeId typeId = this.QueryById<EntityTypeId>(id, out GroupIndex groupIndex);
            EntityDescriptorId descriptorId = this.QueryByGroupIndex<EntityDescriptorId>(in groupIndex);
            
            writer.Write(id.VhId);
            writer.WriteStruct(typeId);
            this.GetDescriptorEngine(descriptorId).Serialize(writer, in groupIndex, in options);
        }

        public EntityId Deserialize(DeserializationOptions options, EntityData data, EntityInitializerDelegate? initializer)
        {
            _reader.Load(data);
            return this.Deserialize(options, _reader, initializer);
        }

        public EntityId Deserialize(DeserializationOptions options, EntityReader reader, EntityInitializerDelegate? initializerDelegate)
        {
            VhId vhid = reader.ReadVhId(options.Seed);
            EntityTypeId typeId = reader.ReadStruct<EntityTypeId>();
            IEntityType type = _types.GetById(typeId);

            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityService), nameof(Deserialize), vhid.Value, typeId.Value, options.Seed.Value);

            SpawnEntity createEntityEvent = new SpawnEntity()
            {
                Type = type,
                VhId = vhid,
                TeamId = options.TeamId,
                Initializer = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    reader.Load(readerState);
                    entities.GetDescriptorEngine(type.Descriptor.Id).Deserialize(in options, reader, ref initializer, in id);

                    initializerDelegate?.Invoke(entities, ref initializer, in id);
                }
            };

            this.Simulation.Publish(vhid, createEntityEvent);

            return this.GetId(vhid);
        }
    }
}
