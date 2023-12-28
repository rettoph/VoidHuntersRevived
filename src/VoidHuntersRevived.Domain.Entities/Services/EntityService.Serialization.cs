using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Events;

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
            Id<IEntityType> typeId = this.QueryById<Id<IEntityType>>(id, out GroupIndex groupIndex);
            Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to serialize {EntityId} of type {EntityType}", nameof(EntityService), nameof(Serialize), id.VhId, typeId.Value);

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
            Id<IEntityType> typeId = reader.ReadStruct<Id<IEntityType>>();
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
