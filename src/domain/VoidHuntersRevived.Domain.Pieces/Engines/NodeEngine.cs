using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Events;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class NodeEngine(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        INodeSocketService socketService,
        ILogger logger) : StrategyEngine,
        IOnSpawnEngine<Node>,
        IOnDespawnEngine<Node>,
        IOnStepEngine
    {
        private readonly INodeSocketService _socketService = socketService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;
        private readonly DictionaryQueue<EntityLocalId, VhId> _dirtyTrees = new();

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node> node)
        {
            _logger.Verbose("OnSpawn - NodeGlobalId = {NodeGlobalId}, LocalTreeId = {LocalTreeId}", node.GlobalId, node.Component.TreeLocalId);

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.Component.TreeLocalId, Tree.NodeFilterContextId);
            filter.Add(node.LocalId, node.Index);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.Component.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, node.GlobalId)
                : HashBuilder<IReactOnAddEx<Node>, EntityGlobalId>.Instance.Calculate(node.GlobalId);

            ref Location treeLocation = ref _entityQueryService.QueryByLocalId<Location>(node.Component.TreeLocalId);
            this.SetLocalTransformation(ref node, in treeLocation);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node> node)
        {
            _logger.Verbose("OnDespawn - NodeId = {NodeId}, TreeLocalId = {TreeLocalId}", node.GlobalId, node.Component.TreeLocalId);

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.Component.TreeLocalId, Tree.NodeFilterContextId);
            filter.Remove(node.LocalId);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.Component.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, node.GlobalId)
                : HashBuilder<IReactOnRemoveEx<Node>, EntityGlobalId>.Instance.Calculate(node.GlobalId);
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            while (_dirtyTrees.TryDequeue(out EntityLocalId dirtyTreeLocalId, out VhId dirtyTreeEventId))
            {
                if (_entityQueryService.IsSpawned(dirtyTreeLocalId))
                {
                    EntityGlobalId dirtyTreGlobalId = _entityQueryService.GetGlobalId(dirtyTreeLocalId);

                    this.Strategy.Publish(dirtyTreeEventId, new Tree_Clean()
                    {
                        IsPrivate = true,
                        TreeGlobalId = dirtyTreGlobalId
                    });
                }
            }
        }

        private void SetLocalTransformation(ref Entity<Node> node, in Location treeLocation)
        {
            _logger.Verbose("Preparing to set {LocalTransformation} for {Node} {NodeId}", nameof(Node.LocalTransformation), nameof(Node), node.LocalId);

            node.Component.SetWorldTransform(treeLocation.ToFixTransform2D());

            if (!_entityQueryService.TryQueryByGroupIndex<Coupling>(node.GroupIndex, out Coupling coupling) || coupling.SocketId == NodeSocketLocalId.Empty)
            {
                node.Component.SetLocationTransform(FixTransform2D.Identity);
                return;
            }

            try
            {
                ref Plug plug = ref _entityQueryService.QueryByGroupIndex<Plug>(node.GroupIndex);
                NodeSocket nodeSocket = _socketService.GetNodeSocket(coupling.SocketId);

                node.Component.SetLocationTransform(FixTransform2D.Invert(plug.Location.ToFixTransform2D()) * nodeSocket.LocalTransform);
            }
            catch (Exception ex)
            {
                // TODO: Investigate what might cause this error
                // When this happens a valid piece gets eaten and destroyed
                // its the opposite of a dupe glitch
                // I can only replicate it by spam clicking the tractor beam selection button and
                // moving the mouse randomly. It doesnt occurre very often
                // We set the transformation to zero so that the constructed rigid shape can still take form
                // Without this it will default all vertices to 0,0 and fail an assert
                node.Component.SetLocationTransform(FixTransform2D.Identity);

                var localId = _entityQueryService.QueryByGroupIndex<EntityLocalId>(node.GroupIndex);
                _logger.Error(ex, "There was a fatal error attempting to set node transformation for node {NodeLocalId}.", localId);
                _entitySpawnService.Despawn(NameSpace<NodeEngine>.Instance, localId);
#if DEBUG
                throw;
#endif
            }
        }
    }
}
