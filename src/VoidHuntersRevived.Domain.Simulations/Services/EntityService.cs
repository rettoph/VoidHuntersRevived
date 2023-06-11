using Guppy.Common.Collections;
using Svelto.ECS;
using Svelto.ECS.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Abstractions;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed class EntityService : IEntityService, IQueryingEntitiesEngine
    {
        private readonly EntityTypeService _entityTypes;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly Dictionary<EntityId, EGIDGroup> _keyMap;
        private readonly Dictionary<EGID, EntityId> _idMap;
        private uint _id;
        private EntitiesDB _entitiesDB;

        public EntitiesDB entitiesDB { set => _entitiesDB = value; }

        public EntityService(
            EntityTypeService entityTypes, 
            IEntityFactory factory,
            IEntityFunctions functions)
        {
            _entityTypes = entityTypes;
            _factory = factory;
            _functions = functions;
            _keyMap = new Dictionary<EntityId, EGIDGroup>();
            _idMap = new Dictionary<EGID, EntityId>();
            _entitiesDB = null!;
        }

        public void Ready()
        {
        }

        public EntityId Create(EntityType type, EntityId id)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);

            _keyMap.Add(id, new EGIDGroup(initializer.EGID, descriptorGroup.Group));
            _idMap.Add(initializer.EGID, id);

            initializer.Get<Component<EntityId>>().Instance = id;

            return id;
        }

        public EntityId Create(EntityType type, EntityId id, Action<IEntityInitializer> initializerAction)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);
            EGIDGroup egidGroup = new EGIDGroup(initializer.EGID, descriptorGroup.Group);

            _keyMap.Add(id, egidGroup);
            _idMap.Add(initializer.EGID, id);

            initializer.Get<Component<EntityId>>().Instance = id;
            initializerAction(new InternalEntityInitializer(_entitiesDB, egidGroup, id, type));

            return id;
        }

        public void Destroy(EntityId id)
        {
            if(!this.TryGetEGIDGroup(ref id, out EGIDGroup egidGroup))
            {
                return;
            }

            _idMap.Remove(egidGroup.EGID);
            _keyMap.Remove(id);
            _functions.RemoveEntity<EntityDescriptor>(egidGroup.EGID);
        }

        public bool TryGetEGIDGroup(ref EntityId key, out EGIDGroup egidGroup)
        {
            return _keyMap.TryGetValue(key, out egidGroup);
        }

        public EntityId GetEntityKey(uint id, ExclusiveGroupStruct group)
        {
            return _idMap[new EGID(id, group)];
        }
    }
}
