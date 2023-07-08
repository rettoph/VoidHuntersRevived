using Guppy.Attributes;
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
using VoidHuntersRevived.Domain.Entities.EnginesGroups;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntitySerializationService : IEntitySerializationService, IQueryingEntitiesEngine
    {
        private readonly EntityReader _reader = new EntityReader();
        private readonly EntityWriter _writer = new EntityWriter();
        private readonly IEventPublishingService _events;
        private readonly IEntityService _entities;
        private readonly IEngineService _engines;
        private readonly EntityTypeService _types;
        private Dictionary<IEntityType, FasterList<SerializationEnginesGroup>> _serializationEngines;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntitySerializationService(
            IEventPublishingService events, 
            IEntityService entities, 
            IEngineService engines,
            EntityTypeService types)
        {
            _events = events;
            _entities = entities;
            _engines = engines;
            _types = types;
            _serializationEngines = null!;
        }

        public void Ready()
        {
            // Create all OnCloneEnginee groups via reflection
            Dictionary<Type, SerializationEnginesGroup> componentCloneEngineGroups = new Dictionary<Type, SerializationEnginesGroup>();
            foreach (IEngine engine in _engines.All())
            {
                foreach (Type serializationEngineType in engine.GetType().GetConstructedGenericTypes(typeof(ISerializationEngine<>)))
                {
                    Type componentType = serializationEngineType.GenericTypeArguments[0];

                    if (componentCloneEngineGroups.ContainsKey(componentType))
                    {
                        continue;
                    }

                    Type serializationEngineTypeGroup = typeof(SerializationEnginesGroup<>).MakeGenericType(componentType);
                    componentCloneEngineGroups.Add(componentType, (SerializationEnginesGroup)Activator.CreateInstance(serializationEngineTypeGroup, _engines)!);
                }
            }

            _serializationEngines = _types.GetAllConfigurations().ToDictionary(
                keySelector: x => x.Type,
                elementSelector: x =>
                {
                    List<SerializationEnginesGroup> serializationEnginesGroups = new List<SerializationEnginesGroup>();
                    foreach (Type componentType in x.Type.Descriptor.ComponentManagers.Select(m => m.Type))
                    {
                        if (componentCloneEngineGroups.TryGetValue(componentType, out var group))
                        {
                            serializationEnginesGroups.Add(group);
                        }
                    }

                    return new FasterList<SerializationEnginesGroup>(serializationEnginesGroups);
                });
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
            writer.WriteUnmanaged(type.Id);

            this.entitiesDB.QueryEntitiesAndIndex<EntityVhId>(id.EGID, out uint index);
            type.Descriptor.Serialize(writer, id.EGID, this.entitiesDB, index);

            foreach (SerializationEnginesGroup serializationEngineGroup in _serializationEngines[type])
            {
                serializationEngineGroup.Serialize(writer, id.EGID, this.entitiesDB, index);
            }
        }

        public IdMap Deserialize(in VhId seed, EntityData data, bool confirmed)
        {
            _reader.Load(data);
            return this.Deserialize(in seed, _reader, confirmed);
        }

        public IdMap Deserialize(in VhId seed, EntityReader reader, bool confirmed)
        {
            VhId vhid = seed.Create(reader.ReadVhId());
            VhId typeId = reader.ReadVhId();

            IEntityType type = _types.GetById(typeId);
            EntityReaderState readerState = reader.GetState(in seed);

            EventDto createEvent = CreateEntity.CreateEvent(type, vhid, (ref EntityInitializer initializer) =>
            {
                reader.Load(readerState);
                type.Descriptor.Deserialize(in readerState.Seed, reader, ref initializer);

                foreach (SerializationEnginesGroup serializationEngineGroup in _serializationEngines[type])
                {
                    serializationEngineGroup.Deserialize(in readerState.Seed, reader, ref initializer, confirmed);
                }

                reader.Busy = false;
            });

            _events.Publish(createEvent);

            if(confirmed)
            {
                _events.Confirm(createEvent);
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
