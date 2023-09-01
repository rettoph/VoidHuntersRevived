using Svelto.ECS;
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
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Engines;

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
            DescriptorId descriptorId = this.QueryById<DescriptorId>(id, out GroupIndex groupIndex);
            
            writer.Write(id.VhId);
            writer.WriteStruct(descriptorId.Value);
            this.GetDescriptorEngine(descriptorId.Value).Serialize(writer, groupIndex);
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

            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MathodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityService), nameof(Deserialize), vhid.Value, descriptorId.Value, reader.Seed.Value);

            SpawnEntityDescriptor createEntityEvent = new SpawnEntityDescriptor()
            {
                DescriptorId = descriptorId,
                VhId = vhid,
                Initializer = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    reader.Load(readerState);
                    entities.GetDescriptorEngine(descriptorId).Deserialize(reader, ref initializer, in id);

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
