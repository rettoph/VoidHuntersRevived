using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class TreeEngine(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        ILogger logger) : StrategyEngine,
        IOnSpawnEngine<Tree>,
        IOnDespawnEngine<Tree>,
        IOnStepEngine
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tree> tree)
        {
            ref Location transform = ref _entityQueryService.QueryByGroupIndex<Location>(tree.GroupIndex);
            ref var filter = ref _entityQueryService.GetFilter<Node>(tree.LocalId, Tree.NodeFilterContextId);

            this.TransformNodes(ref transform, ref filter);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tree> tree)
        {
            _logger.Verbose("Despawning Tree {TreeId}, HeadLocalId = {HeadLocalId}", tree.LocalId, tree.Component.HeadLocalId);
            _entitySpawnService.Despawn(sourceEventId, tree.Component.HeadLocalId);
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, locations, enableds, awakes, count), _) in _entityQueryService.QueryEntities<EntityLocalId, Location, Enabled, Awake>())
            {
                for (uint treeIndex = 0; treeIndex < count; treeIndex++)
                {
                    if (enableds[treeIndex] == false || awakes[treeIndex] == false)
                    {
                        continue;
                    }

                    ref var filter = ref _entityQueryService.GetFilter<Node>(localIds[treeIndex], Tree.NodeFilterContextId);
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
                    nodes[indices[i]].SetWorldTransform(location.Transform);
                }
            }
        }
    }
}
