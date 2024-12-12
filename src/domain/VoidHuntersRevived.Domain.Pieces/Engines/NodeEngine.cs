using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
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
        ISocketService socketService,
        ILogger logger) : StrategyEngine,
        IOnSpawnEngine<Node>,
        IOnDespawnEngine<Node>,
        IOnStepEngine
    {
        private readonly ISocketService _socketService = socketService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;
        private readonly DictionaryQueue<EntityId, VhId> _dirtyTrees = new();

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node> node)
        {
            _logger.Verbose("OnSpawn - NodeId = {NodeId}, TreeId = {TreeId}, LocalTreeId = {LocalTreeId}", node.GlobalId, node.Value.TreeId.VhId, node.Value.TreeId.ToLocalEntityId());

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.Value.TreeId, Tree.NodeFilterContextId);
            filter.Add(node.LocalId, node.Index);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.Value.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Value.Id.VhId)
                : HashBuilder<IReactOnAddEx<Node>, VhId>.Instance.Calculate(node.Value.Id.VhId);

            ref Location treeLocation = ref _entityQueryService.QueryById<Location>(node.Value.TreeId);
            this.SetLocalTransformation(ref node.Value, node.GroupIndex, in treeLocation);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node> node)
        {
            _logger.Verbose("OnDespawn - NodeId = {NodeId}, TreeId = {TreeId}, LocalTreeId = {LocalTreeId}", node.GlobalId, node.Value.TreeId.VhId, node.Value.TreeId.ToLocalEntityId());

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.Value.TreeId, Tree.NodeFilterContextId);
            filter.Remove(node.LocalId);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.Value.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Value.Id.VhId)
                : HashBuilder<IReactOnRemoveEx<Node>, VhId>.Instance.Calculate(node.Value.Id.VhId);
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            while (_dirtyTrees.TryDequeue(out EntityId dirtyTreeId, out VhId dirtyTreeEventId))
            {
                if (_entityQueryService.IsSpawned(dirtyTreeId))
                {
                    this.Strategy.Publish(dirtyTreeEventId, new Tree_Clean()
                    {
                        IsPrivate = true,
                        TreeId = dirtyTreeId.VhId
                    });
                }
            }
        }

        private void SetLocalTransformation(ref Node node, in GroupIndex groupIndex, in Location treeLocation)
        {
            _logger.Verbose("Preparing to set {LocalTransformation} for {Node} {NodeId}", nameof(Node.LocalLocation), nameof(Node), node.Id.VhId.Value);

            node.WorldTransform(treeLocation.Transformation);

            if (!_entityQueryService.TryQueryByGroupIndex<Coupling>(groupIndex, out Coupling coupling) || coupling.SocketId == NodeSocketId.Empty)
            {
                node.SetLocationTransformation(FixMatrix.Identity);
                return;
            }

            try
            {
                ref Plug plug = ref _entityQueryService.QueryByGroupIndex<Plug>(groupIndex);
                NodeSocket nodeSocket = _socketService.GetSocket(coupling.SocketId);

                node.SetLocationTransformation(plug.Location.Transformation.Invert() * nodeSocket.LocalTransformation);
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
                node.SetLocationTransformation(FixMatrix.Identity);

                var id = _entityQueryService.QueryByGroupIndex<EntityId>(groupIndex);
                _logger.Error(ex, "There was a fatal error attempting to set node transformation for node {NodeId}.", id.VhId.Value);
                _entitySpawnService.Despawn(NameSpace<NodeEngine>.Instance, id);
            }
        }
    }
}
