using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityService : IEntityService,
        IReactOnAddAndRemoveEx<EntityVhId>
    {
        private readonly EntityConfigurationService _entityConfigurations;
        private readonly EntityPropertyService _properties;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly Dictionary<VhId, EGID> _vhidMap;
        private readonly Dictionary<EGID, VhId> _egidMap;
        private uint _id;

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
            _vhidMap = new Dictionary<VhId, EGID>();
            _egidMap = new Dictionary<EGID, VhId>();
        }

        public void Ready()
        {
        }

        public VhId Create(EntityName name, VhId id)
        {
            EntityConfiguration configuration = _entityConfigurations.GetConfiguration(name);
            EntityInitializer initializer = _factory.BuildEntity(_id++, configuration.typeConfiguration.group, configuration.typeConfiguration.descriptor);

            initializer.Get<EntityVhId>().Value = id;

            foreach(PropertyCache property in configuration.properties)
            {
                property.Initialize(ref initializer);
            }

            return id;
        }

        public VhId Create(EntityName name, VhId id, EntityInitializerDelegate initializerDelegate)
        {
            EntityConfiguration configuration = _entityConfigurations.GetConfiguration(name);
            EntityInitializer initializer = _factory.BuildEntity(_id++, configuration.typeConfiguration.group, configuration.typeConfiguration.descriptor);

            initializer.Get<EntityVhId>().Value = id;
            initializerDelegate(ref initializer);

            return id;
        }

        public void Destroy(VhId id)
        {
            if(!this.TryGetEGID(ref id, out EGID egid))
            {
                return;
            }

            _egidMap.Remove(egid);
            _vhidMap.Remove(id);
            _functions.RemoveEntity<EntityDescriptor>(egid);
        }

        public T GetProperty<T>(Property<T> id)
            where T : class, IEntityProperty
        {
            return _properties.GetProperty(in id);
        }

        public bool TryGetEGID(ref VhId id, out EGID egid)
        {
            return _vhidMap.TryGetValue(id, out egid);
        }

        public VhId GetEntityId(uint id, ExclusiveGroupStruct group)
        {
            return _egidMap[new EGID(id, group)];
        }

        public EGID GetEGID(VhId id)
        {
            return _vhidMap[id];
        }

        public VhId GetVhId(EGID egid)
        {
            return _egidMap[egid];
        }

        public VhId GetVhId(uint id, ExclusiveGroupStruct group)
        {
            return this.GetVhId(new EGID(id, group));
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<EntityVhId> entities, ExclusiveGroupStruct groupID)
        {
            var (entityIds, nativeIds, _) = entities;

            //for each entity added in this submission phase
            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                //get the Stride entityID that will be instanced multipled times
                VhId id = entityIds[index].Value;
                EGID egid = new EGID(nativeIds[index], groupID);

                _vhidMap.Add(id, egid);
                _egidMap.Add(egid, id);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<EntityVhId> entities, ExclusiveGroupStruct groupID)
        {
            var (entityIds, nativeIds, _) = entities;

            //for each entity added in this submission phase
            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                //get the Stride entityID that will be instanced multipled times
                VhId id = entityIds[index].Value;
                EGID egid = new EGID(nativeIds[index], groupID);

                _vhidMap.Remove(id);
                _egidMap.Remove(egid);
            }
        }
    }
}
