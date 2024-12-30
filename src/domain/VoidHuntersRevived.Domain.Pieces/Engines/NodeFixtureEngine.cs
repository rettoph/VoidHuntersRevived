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
    public sealed class NodeFixtureEngine(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        INodeSocketService socketService,
        ILogger logger) : StrategyEngine,
            IOnSpawnEngine<Node, Fixture>,
            IOnDespawnEngine<Node, Fixture>,
            IOnStepEngine
    {
        private readonly INodeSocketService _socketService = socketService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;
        private readonly DictionaryQueue<EntityLocalId, VhId> _dirtyTrees = new();

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group02)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<Node, Fixture> entity)
        {
            _logger.Verbose("OnSpawn - NodeGlobalId = {NodeGlobalId}", entity.GlobalId);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(entity.First.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, entity.GlobalId)
                : HashBuilder<IReactOnAddEx<Node>, EntityGlobalId>.Instance.Calculate(entity.GlobalId);

            ref Body body = ref _entityQueryService.QueryByEGID<Body>(entity.Second.BodyFilterId.EGID);
            this.SetLocalTransformation(ref entity, in body);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node, Fixture> entity)
        {
            _logger.Verbose("OnSpawn - NodeGlobalId = {NodeGlobalId}", entity.GlobalId);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(entity.First.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, entity.GlobalId)
                : HashBuilder<IReactOnRemoveEx<Node>, EntityGlobalId>.Instance.Calculate(entity.GlobalId);
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

        private void SetLocalTransformation(ref Entity<Node, Fixture> node, in Body treeBody)
        {
            _logger.Verbose("Preparing to set {LocalTransformation} for {Node} {Fixture} {NodeId}", nameof(Fixture.LocalTransform), nameof(Node), nameof(Fixture), node.LocalId);

            node.Second.SetBodyTransform(treeBody.Transform);

            if (!_entityQueryService.TryQueryByGroupIndex<Coupling>(node.GroupIndex, out Coupling coupling) || coupling.SocketId == NodeSocketLocalId.Empty)
            {
                node.Second.SetLocalRotationTransform(Fix64.Zero, FixTransform2D.Identity);
                return;
            }

            try
            {
                ref Plug plug = ref _entityQueryService.QueryByGroupIndex<Plug>(node.GroupIndex);
                NodeSocket nodeSocket = _socketService.GetNodeSocket(coupling.SocketId);

                FixTransform2D localTransform = FixTransform2D.Invert(plug.NodeTransform) * nodeSocket.LocalTransform;
                node.Second.SetLocalRotationTransform(localTransform.Rotation.Phase, localTransform);
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
                node.Second.SetLocalRotationTransform(Fix64.Zero, FixTransform2D.Identity);

                var localId = _entityQueryService.QueryByGroupIndex<EntityLocalId>(node.GroupIndex);
                _logger.Error(ex, "There was a fatal error attempting to set node transformation for node {NodeLocalId}.", localId);
                _entitySpawnService.Despawn(NameSpace<NodeFixtureEngine>.Instance, localId);
#if DEBUG
                throw;
#endif
            }
        }
    }
}
