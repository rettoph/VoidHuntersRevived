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

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly EntityReader _reader = new EntityReader();
        private readonly EntityWriter _writer = new EntityWriter();

        public EntityData Serialize(EntityId id)
        {
            _writer.Reset();
            this.Serialize(id, _writer);
            return _writer.Export(id.VhId);
        }

        public void Serialize(EntityId id, EntityWriter writer)
        {
            ScopedVoidHuntersEntityDescriptor scopedDescriptor = _descriptors.Value.GetByEntityVhId(id.VhId);

            writer.Write(id.VhId);
            writer.WriteStruct(scopedDescriptor.GlobalDescriptor.Id);

            this.entitiesDB.QueryEntitiesAndIndex<EntityId>(id.EGID, out uint index);
            scopedDescriptor.Serialize(writer, id.EGID, this.entitiesDB, index);
        }

        public EntityId Deserialize(in VhId seed, EntityData data, EntityInitializerDelegate? initializer, bool confirmed)
        {
            _reader.Load(seed, data, confirmed);
            return this.Deserialize(_reader, initializer);
        }

        public EntityId Deserialize(EntityReader reader, EntityInitializerDelegate? initializerDelegate)
        {
            VhId vhid = reader.ReadVhId();
            VhId descriptorId = reader.ReadStruct<VhId>();

            ScopedVoidHuntersEntityDescriptor scopedDescriptor = _descriptors.Value.GetById(descriptorId);
            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MathodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityService), nameof(Deserialize), vhid.Value, scopedDescriptor.GlobalDescriptor.Name, reader.Seed.Value);

            SpawnEntityDescriptor createEntityEvent = new SpawnEntityDescriptor()
            {
                Descriptor = scopedDescriptor.GlobalDescriptor,
                VhId = vhid,
                Initializer = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    reader.Load(readerState);
                    scopedDescriptor.Deserialize(reader, ref initializer, in id);

                    initializerDelegate?.Invoke(entities, ref initializer, in id);

                    reader.Busy = false;
                }
            };

            this.Simulation.Publish(vhid, createEntityEvent);

            return this.GetId(vhid);
        }

        public EntityData Serialize(VhId vhid)
        {
            return this.Serialize(this.GetId(vhid));
        }

        public EntityData Serialize(EGID egid)
        {
            return this.Serialize(this.GetId(egid));
        }

        public EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.Serialize(this.GetId(entityId, groupId));
        }

        public void Serialize(VhId vhid, EntityWriter writer)
        {
            this.Serialize(this.GetId(vhid), writer);
        }

        public void Serialize(EGID egid, EntityWriter writer)
        {
            this.Serialize(this.GetId(egid), writer);
        }

        public void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer)
        {
            this.Serialize(this.GetId(entityId, groupId), writer);
        }
    }
}
