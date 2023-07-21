﻿using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using Svelto.ECS.Serialization;
using System.Text;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<StepSequence>(StepSequence.OnEntitySubmit)]
    internal sealed partial class EntityIdService : IEntityIdService, IEngine, IStepEngine<Step>
    {
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;
        private readonly DoubleDictionary<VhId, EGID, EntityId> _ids;
        private readonly Dictionary<VhId, VoidHuntersEntityDescriptor> _descriptors;
        private readonly Queue<EntityId> _removed;

        public string name { get; } = nameof(EntityIdService);

        public EntityIdService(SimpleEntitiesSubmissionScheduler scheduler)
        {
            _ids = new DoubleDictionary<VhId, EGID, EntityId>();
            _descriptors = new Dictionary<VhId, VoidHuntersEntityDescriptor>();
            _removed = new Queue<EntityId>();
            _scheduler = scheduler;
        }

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public EntityId GetId(EGID egid)
        {
            return _ids[egid];
        }

        public EntityId GetId(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.GetId(new EGID(entityId, groupId));
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGet(vhid, out id);
        }

        public bool TryGetId(EGID egid, out EntityId id)
        {
            return _ids.TryGet(egid, out id);
        }

        public bool TryGetId(uint entityId, ExclusiveGroupStruct groupId, out EntityId id)
        {
            return _ids.TryGet(new EGID(entityId, groupId), out id);
        }

        public VoidHuntersEntityDescriptor GetEntityDescriptor(VhId entityVhId)
        {
            return _descriptors[entityVhId];
        }

        public void Clean()
        {
            while(_removed.TryDequeue(out EntityId removed))
            {
                _ids.Remove(removed.VhId, removed.EGID);
                _descriptors.Remove(removed.VhId);
            }
        }

        public void Step(in Step _param)
        {
            _scheduler.SubmitEntities();
        }

        internal EntityId Add(VhId vhid, EGID egid, VoidHuntersEntityDescriptor descriptor)
        {
            EntityId idMap = new EntityId(egid, vhid);
            _ids.TryAdd(vhid, egid, idMap);
            _descriptors.Add(vhid, descriptor);

            return idMap;
        }

        internal EntityId Remove(VhId vhid, out VoidHuntersEntityDescriptor descriptor)
        {
            ref EntityId id = ref _ids.TryGetRef(vhid, out bool isNullRef);
            if (isNullRef)
            {
                throw new NullReferenceException();
            }

            id.Destroyed = true;
            _removed.Enqueue(id);
            descriptor = _descriptors[vhid];

            return id;
        }
    }
}
