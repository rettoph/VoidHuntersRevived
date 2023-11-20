﻿using Guppy.Attributes;
using Guppy.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.PreStep)]
    internal sealed class TreeEngine : BasicEngine,
        IOnSpawnEngine<Tree>,
        IOnDespawnEngine<Tree>,
        IStepEngine<Step>
    {
        public string name { get; } = nameof(TreeEngine);

        private HashSet<EGID> _removedNodes = new HashSet<EGID>();
        private readonly IEntityService _entities;
        private ITeamDescriptorGroupService _teamDescriptorGroups;

        public TreeEngine(IEntityService entities, ITeamDescriptorGroupService teamDescriptorGroupService)
        {
            _entities = entities;
            _teamDescriptorGroups = teamDescriptorGroupService;
        }

        public void OnDespawn(EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            _entities.Despawn(component.HeadId);
        }

        public void OnSpawn(EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            if(_teamDescriptorGroups.GetByGroupId(groupIndex.GroupID).Descriptor is not TreeDescriptor)
            {
                throw new Exception();
            }

            ref Location location = ref _entities.QueryByGroupIndex<Location>(groupIndex);
            ref var filter = ref _entities.GetFilter<Node>(id, Tree.NodeFilterContextId);

            this.TransformNodes(ref location, ref filter);
        }

        public void Step(in Step _param)
        {
            var groups = _entities.FindGroups<Tree, Location, Enabled, Awake>();
            foreach (var ((ids, locations, enableds, awakes, count), _) in _entities.QueryEntities<EntityId, Location, Enabled, Awake>(groups))
            {
                for (uint treeIndex = 0; treeIndex < count; treeIndex++)
                {
                    if (enableds[treeIndex] == false || awakes[treeIndex] == false)
                    {
                        continue;
                    }

                    ref var filter = ref _entities.GetFilter<Node>(ids[treeIndex], Tree.NodeFilterContextId);
                    this.TransformNodes(ref locations[treeIndex], ref filter);
                }
            }
        }

        private void TransformNodes(ref Location location, ref EntityFilterCollection filter)
        {
            foreach (var (indices, group) in filter)
            {
                var (nodes, _) = _entities.QueryEntities<Node>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    nodes[indices[i]].WorldTransform(location.Transformation);
                }
            }
        }
    }
}
