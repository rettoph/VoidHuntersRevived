using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class TreeEngine(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        ILogger logger) : StrategyEngine,
        IOnSpawnEngine<Tree>,
        IOnDespawnEngine<Tree>,
        IOnStepEngine
    {

        private HashSet<EGID> _removedNodes = [];
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;

        public void OnSpawn(VhId sourceEventId, IEntityTemplate type, EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            ref Location location = ref _entityQueryService.QueryByGroupIndex<Location>(groupIndex);
            ref var filter = ref _entityQueryService.GetFilter<Node>(id, Tree.NodeFilterContextId);

            this.TransformNodes(ref location, ref filter);
        }

        public void OnDespawn(VhId sourceEventId, IEntityTemplate type, EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Despawning Tree {TreeId}, HeadId = {HeadId}", nameof(TreeEngine), nameof(OnDespawn), id.VhId, component.HeadId.VhId);
            _entitySpawnService.Despawn(sourceEventId, component.HeadId);
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            var groups = _entityQueryService.FindGroups<Tree, Location, Enabled, Awake>();
            foreach (var ((ids, locations, enableds, awakes, count), _) in _entityQueryService.QueryEntities<EntityId, Location, Enabled, Awake>(groups))
            {
                for (uint treeIndex = 0; treeIndex < count; treeIndex++)
                {
                    if (enableds[treeIndex] == false || awakes[treeIndex] == false)
                    {
                        continue;
                    }

                    ref var filter = ref _entityQueryService.GetFilter<Node>(ids[treeIndex], Tree.NodeFilterContextId);
                    this.TransformNodes(ref locations[treeIndex], ref filter);
                }
            }
        }

        private void TransformNodes(ref Location location, ref EntityFilterCollection filter)
        {
            foreach (var (indices, group) in filter)
            {
                var (nodes, _) = _entityQueryService.QueryEntities<Node>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    nodes[indices[i]].WorldTransform(location.Transformation);
                }
            }
        }
    }
}
