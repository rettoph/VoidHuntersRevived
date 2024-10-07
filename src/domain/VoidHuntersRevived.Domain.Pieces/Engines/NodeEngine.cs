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
    [AutoLoad]
    internal sealed class NodeEngine(
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
        private readonly DictionaryQueue<EntityId, VhId> _dirtyTrees = new DictionaryQueue<EntityId, VhId>();

        public void OnSpawn(VhId sourceEventId, IEntityType type, EntityId id, ref Node node, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(NodeEngine), nameof(OnSpawn), id.VhId);

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.TreeId, Tree.NodeFilterContextId);
            filter.Add(id, groupIndex);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Id.VhId)
                : HashBuilder<IReactOnAddEx<Node>, VhId>.Instance.Calculate(node.Id.VhId);

            ref Location treeLocation = ref _entityQueryService.QueryById<Location>(node.TreeId);
            this.SetLocalTransformation(ref node, groupIndex, in treeLocation);
        }

        public void OnDespawn(VhId sourceEventId, IEntityType type, EntityId id, ref Node node, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(NodeEngine), nameof(OnDespawn), id.VhId);

            ref var filter = ref _entityQueryService.GetFilter<Node>(node.TreeId, Tree.NodeFilterContextId);
            filter.Remove(id.EGID);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Id.VhId)
                : HashBuilder<IReactOnRemoveEx<Node>, VhId>.Instance.Calculate(node.Id.VhId);
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
            _logger.Verbose("{ClassName}::{MethodName} - Preparing to set {LocalTransformation} for {Node} {NodeId}", nameof(NodeEngine), nameof(SetLocalTransformation), nameof(Node.LocalLocation), nameof(Node), node.Id.VhId.Value);

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
                _logger.Error(ex, "{ClassName}::{MethodName} - There was a fatal error attempting to set node transformation for node {NodeId}.", nameof(NodeEngine), nameof(SetLocalTransformation), id.VhId.Value);
                _entitySpawnService.Despawn(NameSpace<NodeEngine>.Instance, id);
            }
        }
    }
}
