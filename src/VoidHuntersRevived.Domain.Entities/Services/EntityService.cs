using Guppy.Common.Collections;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityService : IEntityService,
        IReactOnAddAndRemoveEx<EntityVhId>, IQueryingEntitiesEngine
    {
        private readonly EntityConfigurationService _entityConfigurations;
        private readonly EntityPropertyService _properties;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _idMap;
        private uint _id;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(
            EntityConfigurationService entityConfigurations,
            EntityPropertyService properties, 
            IEntityFactory factory,
            IEntityFunctions functions)
        {
            _entityConfigurations = entityConfigurations;
            _properties = properties;
            _factory = factory;
            _functions = functions;
            _idMap = new DoubleDictionary<VhId, EGID, IdMap>();
        }

        public void Ready()
        {
        }

        public IdMap Create(EntityName name, VhId vhid)
        {
            EntityConfiguration configuration = _entityConfigurations.GetConfiguration(name);
            EntityInitializer initializer = _factory.BuildEntity(_id++, configuration.typeConfiguration.group, configuration.typeConfiguration.descriptor);

            initializer.Get<EntityVhId>().Value = vhid;

            foreach(PropertyCache property in configuration.properties)
            {
                property.Initialize(ref initializer);
            }

            return new IdMap(initializer.EGID, vhid);
        }

        public IdMap Create(EntityName name, VhId vhid, EntityInitializerDelegate initializerDelegate)
        {
            EntityConfiguration configuration = _entityConfigurations.GetConfiguration(name);
            EntityInitializer initializer = _factory.BuildEntity(_id++, configuration.typeConfiguration.group, configuration.typeConfiguration.descriptor);

            initializer.Get<EntityVhId>().Value = vhid;

            foreach (PropertyCache property in configuration.properties)
            {
                property.Initialize(ref initializer);
            }

            initializerDelegate(ref initializer);

            return new IdMap(initializer.EGID, vhid);
        }

        public void Destroy(VhId vhid)
        {
            if(!this.TryGetIdMap(ref vhid, out IdMap id))
            {
                return;
            }

            _functions.RemoveEntity<EntityDescriptor>(id.EGID);
        }

        public T GetProperty<T>(Property<T> id)
            where T : class, IEntityProperty
        {
            return _properties.GetProperty(in id);
        }

        public bool TryGetProperty<T>(EGID id, out T property) 
            where T : class, IEntityProperty
        {
            if(this.entitiesDB.TryGetEntity(id, out Property<T> propertyComponent))
            {
                property = _properties.GetProperty(propertyComponent);
                return true;
            }

            property = default!;
            return false;
        }

        public IEnumerable<(int, T)> GetProperties<T>()
            where T : class, IEntityProperty
        {
            return _properties.GetProperties<T>();
        }

        public bool TryGetIdMap(ref VhId vhid, out IdMap id)
        {
            return _idMap.TryGet(vhid, out id);
        }

        public IdMap GetIdMap(VhId vhid)
        {
            return _idMap[vhid];
        }

        public IdMap GetIdMap(EGID egid)
        {
            return _idMap[egid];
        }

        public IdMap GetIdMap(uint id, ExclusiveGroupStruct group)
        {
            return this.GetIdMap(new EGID(id, group));
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<EntityVhId> entities, ExclusiveGroupStruct groupID)
        {
            var (entityIds, nativeIds, _) = entities;

            //for each entity added in this submission phase
            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                //get the Stride entityID that will be instanced multipled times
                VhId vhid = entityIds[index].Value;
                EGID egid = new EGID(nativeIds[index], groupID);

                _idMap.TryAdd(vhid, egid, new IdMap(egid, vhid));
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<EntityVhId> entities, ExclusiveGroupStruct groupID)
        {
            var (entityIds, nativeIds, _) = entities;

            //for each entity added in this submission phase
            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                //get the Stride entityID that will be instanced multipled times
                VhId vhid = entityIds[index].Value;
                EGID egid = new EGID(nativeIds[index], groupID);

                _idMap.Remove(vhid, egid);
            }
        }
    }
}
