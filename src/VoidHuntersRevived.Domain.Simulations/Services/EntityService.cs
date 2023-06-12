﻿using Guppy.Common.Collections;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Abstractions;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed class EntityService : IEntityService, IQueryingEntitiesEngine
    {
        private readonly SimpleEntitiesSubmissionScheduler _simpleEntitiesSubmissionScheduler;
        private readonly EntityTypeService _entityTypes;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly Dictionary<Guid, EGIDGroup> _keyMap;
        private readonly Dictionary<EGID, Guid> _idMap;
        private uint _id;
        private EntitiesDB _entitiesDB;

        public EntitiesDB entitiesDB { set => _entitiesDB = value; }

        public EntityService(
            SimpleEntitiesSubmissionScheduler simpleEntitiesSubmissionScheduler,
            EntityTypeService entityTypes, 
            IEntityFactory factory,
            IEntityFunctions functions)
        {
            _simpleEntitiesSubmissionScheduler = simpleEntitiesSubmissionScheduler;
            _entityTypes = entityTypes;
            _factory = factory;
            _functions = functions;
            _keyMap = new Dictionary<Guid, EGIDGroup>();
            _idMap = new Dictionary<EGID, Guid>();
            _entitiesDB = null!;
        }

        public void Ready()
        {
        }

        public Guid Create(EntityType type, Guid id)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);

            _keyMap.Add(id, new EGIDGroup(initializer.EGID, descriptorGroup.Group));
            _idMap.Add(initializer.EGID, id);

            initializer.Get<Component<Guid>>().Instance = id;

            _simpleEntitiesSubmissionScheduler.SubmitEntities();

            return id;
        }

        public Guid Create(EntityType type, Guid id, Action<IEntityInitializer> initializerAction)
        {
            EntityDescriptorGroup descriptorGroup = _entityTypes.EntityDescriptorGroup(type);
            EntityInitializer initializer = _factory.BuildEntity(_id++, descriptorGroup.Group, descriptorGroup.Descriptor);
            EGIDGroup egidGroup = new EGIDGroup(initializer.EGID, descriptorGroup.Group);

            _keyMap.Add(id, egidGroup);
            _idMap.Add(initializer.EGID, id);

            initializer.Get<Component<Guid>>().Instance = id;

            _simpleEntitiesSubmissionScheduler.SubmitEntities();
            initializerAction(new InternalEntityInitializer(_entitiesDB, egidGroup, id, type));

            return id;
        }

        public void Destroy(Guid id)
        {
            if(!this.TryGetEGIDGroup(ref id, out EGIDGroup egidGroup))
            {
                return;
            }

            _idMap.Remove(egidGroup.EGID);
            _keyMap.Remove(id);
            _functions.RemoveEntity<EntityDescriptor>(egidGroup.EGID);

            _simpleEntitiesSubmissionScheduler.SubmitEntities();
        }

        public bool TryGetEGIDGroup(ref Guid key, out EGIDGroup egidGroup)
        {
            return _keyMap.TryGetValue(key, out egidGroup);
        }

        public Guid GetEntityKey(uint id, ExclusiveGroupStruct group)
        {
            return _idMap[new EGID(id, group)];
        }
    }
}
