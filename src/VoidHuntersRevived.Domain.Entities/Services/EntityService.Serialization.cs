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
            DescriptorId descriptorId = this.QueryById<DescriptorId>(id, out GroupIndex groupIndex);
            
            writer.Write(id.VhId);
            writer.WriteStruct(descriptorId.Value);
            this.GetDescriptorEngine(descriptorId.Value).Serialize(writer, in groupIndex, in options);
        }

        public EntityId Deserialize(DeserializationOptions options, EntityData data, EntityInitializerDelegate? initializer)
        {
            _reader.Load(data);
            return this.Deserialize(options, _reader, initializer);
        }

        public EntityId Deserialize(DeserializationOptions options, EntityReader reader, EntityInitializerDelegate? initializerDelegate)
        {
            VhId vhid = reader.ReadVhId(options.Seed);
            VhId descriptorId = reader.ReadStruct<VhId>();

            EntityReaderState readerState = reader.GetState();

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityService), nameof(Deserialize), vhid.Value, descriptorId.Value, options.Seed.Value);

            SpawnEntityDescriptor createEntityEvent = new SpawnEntityDescriptor()
            {
                DescriptorId = descriptorId,
                VhId = vhid,
                TeamId = options.TeamId,
                Initializer = (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    reader.Load(readerState);
                    entities.GetDescriptorEngine(descriptorId).Deserialize(in options, reader, ref initializer, in id);

                    initializerDelegate?.Invoke(entities, ref initializer, in id);
                }
            };

            this.Simulation.Publish(vhid, createEntityEvent);

            return this.GetId(vhid);
        }
    }
}
