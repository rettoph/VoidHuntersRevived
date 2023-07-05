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
    internal sealed class EntitySerializationService : IEntitySerializationService, IEnginesGroupEngine, IQueryingEntitiesEngine
    {
        private readonly EntityTypeService _types;
        private Dictionary<EntityType, FasterList<SerializationEnginesGroup>> _serializationEngines;
        private IEventPublishingService _events;
        private IEntityService _entities;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntitySerializationService(EntityTypeService types)
        {
            _types = types;
            _serializationEngines = null!;
            _events = null!;
            _entities = null!;
        }

        public void Ready()
        {
        }

        public void Initialize(IEngineService engines)
        {
            _events = engines.Events;
            _entities = engines.Entities;

            // Create all OnCloneEnginee groups via reflection
            Dictionary<Type, SerializationEnginesGroup> componentCloneEngineGroups = new Dictionary<Type, SerializationEnginesGroup>();
            foreach (IEngine engine in engines.All())
            {
                foreach (Type serializationEngineType in engine.GetType().GetConstructedGenericTypes(typeof(ISerializationEngine<>)))
                {
                    Type componentType = serializationEngineType.GenericTypeArguments[0];

                    if (componentCloneEngineGroups.ContainsKey(componentType))
                    {
                        continue;
                    }

                    Type serializationEngineTypeGroup = typeof(SerializationEnginesGroup<>).MakeGenericType(componentType);
                    componentCloneEngineGroups.Add(componentType, (SerializationEnginesGroup)Activator.CreateInstance(serializationEngineTypeGroup, engines)!);
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
            EntityWriter.Instance.Reset();
            this.Serialize(id, EntityWriter.Instance);
            return EntityWriter.Instance.Export();
        }

        public void Serialize(IdMap id, EntityWriter writer)
        {
            EntityType type = _entities.GetEntityType(id.VhId);

            writer.Write(id.VhId);
            writer.WriteUnmanaged(type.Id);

            this.entitiesDB.QueryEntitiesAndIndex<EntityVhId>(id.EGID, out uint index);
            type.Descriptor.Serialize(writer, id.EGID, this.entitiesDB, index);

            foreach (SerializationEnginesGroup serializationEngineGroup in _serializationEngines[type])
            {
                serializationEngineGroup.Serialize(writer, id.EGID, this.entitiesDB, index);
            }
        }

        public IdMap Deserialize(VhId seed, EntityData data)
        {
            EntityReader.Instance.Load(seed, data);
            return this.Deserialize(EntityReader.Instance);
        }

        public IdMap Deserialize(EntityReader reader)
        {
            VhId vhid = reader.ReadVhId();
            VhId typeId = reader.ReadUnmanaged<VhId>();
            EntityType type = _types.GetById(typeId);
            EntityReaderState readerState = reader.GetState();

            _events.Publish(CreateEntity.CreateEvent(type, vhid, (ref EntityInitializer initializer) =>
            {
                reader.Load(readerState);
                type.Descriptor.Deserialize(reader, ref initializer);

                foreach (SerializationEnginesGroup serializationEngineGroup in _serializationEngines[type])
                {
                    serializationEngineGroup.Deserialize(reader, ref initializer);
                }
            }));

            return _entities.GetIdMap(vhid);
        }
    }
}
