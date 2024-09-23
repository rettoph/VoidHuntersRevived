using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    [SequenceGroup<StepSequence>(StepSequence.PreStep)]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    internal sealed class TreeEngine : StrategyEngine,
        IOnSpawnEngine<Tree>,
        IOnDespawnEngine<Tree>,
        IStepEngine<Step>
    {
        public string name { get; } = nameof(TreeEngine);

        private HashSet<EGID> _removedNodes = new HashSet<EGID>();
        private readonly IEntityQueryService _entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService;
        private readonly ILogger _logger;

        public TreeEngine(
            IEntityQueryService entityQueryService,
            IEntitySpawnService entitySpawnService,
            ILogger logger)
        {
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
            _logger = logger;
        }

        public void OnSpawn(VhId sourceEventId, IEntityType type, EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            ref Location location = ref _entityQueryService.QueryByGroupIndex<Location>(groupIndex);
            ref var filter = ref _entityQueryService.GetFilter<Node>(id, Tree.NodeFilterContextId);

            this.TransformNodes(ref location, ref filter);
        }

        public void OnDespawn(VhId sourceEventId, IEntityType type, EntityId id, ref Tree component, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Despawning Tree {TreeId}, HeadId = {HeadId}", nameof(TreeEngine), nameof(OnDespawn), id.VhId, component.HeadId.VhId);
            _entitySpawnService.Despawn(sourceEventId, component.HeadId);
        }


        public void Step(in Step _param)
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
