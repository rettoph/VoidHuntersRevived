using Guppy.Common.Collections;
using Svelto.ECS;
using Svelto.ECS.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Services;
using VoidHuntersRevived.Domain.ECS.Abstractions;

namespace VoidHuntersRevived.Domain.ECS.Services
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

        public EntityId Create(EntityType type, EntityId entityKey)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);

            _keyMap.Add(entityKey, new EGIDGroup(initializer.EGID, descriptorGroup.Group));
            _idMap.Add(initializer.EGID, entityKey);

            return entityKey;
        }

        public EntityId Create(EntityType type, EntityId entityKey, Action<IEntityInitializer> initializerAction)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);
            EGIDGroup egidGroup = new EGIDGroup(initializer.EGID, descriptorGroup.Group);

            _keyMap.Add(entityKey, egidGroup);
            _idMap.Add(initializer.EGID, entityKey);

            initializerAction(new InternalEntityInitializer(_entitiesDB, egidGroup, entityKey, type));

            return entityKey;
        }

        public void Destroy(EntityId entityKey)
        {
            if(!this.TryGetEGIDGroup(ref entityKey, out EGIDGroup egidGroup))
            {
                return;
            }

            _idMap.Remove(egidGroup.EGID);
            _keyMap.Remove(entityKey);
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
