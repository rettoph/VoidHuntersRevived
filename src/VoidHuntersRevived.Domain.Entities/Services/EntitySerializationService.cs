using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntitySerializationService : IEntitySerializationService, IQueryingEntitiesEngine
    {
        private readonly EntityService _entities;
        private readonly EntityTypeService _types;

        public EntitySerializationService(EntityService entities, EntityTypeService types)
        {
            _entities = entities;
            _types = types;
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Ready()
        {
        }

        public EntityData Serialize(VhId vhid)
        {
            IdMap id = _entities.GetIdMap(vhid);
            EntityType type = _entities.GetEntityType(vhid);

            EntityData data = new EntityData();
            using (EntityWriter writer = new EntityWriter(data))
            {
                writer.Write(id.VhId);
                writer.WriteUnmanaged(type.Id);

                type.Descriptor.Serialize(writer, id.EGID, this.entitiesDB);
                return data;
            }
        }

        public IdMap Deserialize(VhId seed, EntityData data)
        {
            using (EntityReader reader = new EntityReader(seed, data))
            {
                data.Position = 0;

                VhId vhid = reader.ReadVhId();
                Console.WriteLine(vhid.Value.ToString() + " ----- " + seed.Value.ToString());
                EntityType type = _types.GetById(reader.ReadUnmanaged<VhId>());

                return _entities.Create(type, vhid, (ref EntityInitializer initializer) =>
                {
                    type.Descriptor.Deserialize(reader, ref initializer);
                });
            }
        }

        /*            IdMap sourceId = _ids[sourceVhId];
            EntityType type = _types[sourceVhId];
            EntityInitializer initializer = type.CreateEntity(_factory);
            initializer.Init(new EntityVhId() { Value = cloneId });

            type.Descriptor.Clone(sourceId.EGID, this.entitiesDB, ref initializer);

            IdMap cloneIdMap = new IdMap(initializer.EGID, cloneId);
            _added.Enqueue(cloneIdMap);
            _types.Add(cloneId, type);

            var onCloneEngines = _onCloneEngines[type];
            foreach(OnCloneEnginesGroup engines in onCloneEngines)
            {
                engines.Invoke(in sourceId, in cloneIdMap, ref initializer);
            }

            return cloneIdMap;
        */
    }
}
