using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Events;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public sealed class NodeFixtureSystem(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        INodeSocketService socketService,
        ILogger logger
    ) : ISceneSystem,
        IInitializeSystem<IStrategy>,
        IOnSpawnSystem<Node, Fixture>,
        IOnDespawnSystem<Node, Fixture>,
        IOnStepSystem
    {
        private readonly INodeSocketService _socketService = socketService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;
        private readonly DictionaryQueue<EntityLocalId, VhId> _dirtyTrees = new();
        private IStrategy _strategy = null!;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(IStrategy strategy)
        {
            this._strategy = strategy;
        }

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group02)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<Node, Fixture> entity)
        {
            this._logger.Verbose("OnSpawn - NodeGlobalId = {NodeGlobalId}", entity.GlobalId);

            ref VhId dirtyEventId = ref this._dirtyTrees.GetOrEnqueue(entity.First.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, entity.GlobalId)
                : HashBuilder<IReactOnAddEx<Node>, EntityGlobalId>.Instance.Calculate(entity.GlobalId);

            ref Body body = ref this._entityQueryService.QueryByEGID<Body>(entity.Second.BodyFilterId.EGID);
            this.SetLocalTransformation(ref entity, in body);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Node, Fixture> entity)
        {
            this._logger.Verbose("OnSpawn - NodeGlobalId = {NodeGlobalId}", entity.GlobalId);

            ref VhId dirtyEventId = ref this._dirtyTrees.GetOrEnqueue(entity.First.TreeLocalId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, EntityGlobalId>.Instance.Calculate(dirtyEventId, entity.GlobalId)
                : HashBuilder<IReactOnRemoveEx<Node>, EntityGlobalId>.Instance.Calculate(entity.GlobalId);
        }

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            while (this._dirtyTrees.TryDequeue(out EntityLocalId dirtyTreeLocalId, out VhId dirtyTreeEventId))
            {
                if (this._entityQueryService.IsSpawned(dirtyTreeLocalId))
                {
                    EntityGlobalId dirtyTreGlobalId = this._entityQueryService.GetGlobalId(dirtyTreeLocalId);

                    this._strategy.Publish(dirtyTreeEventId, new Tree_Clean()
                    {
                        IsPrivate = true,
                        TreeGlobalId = dirtyTreGlobalId
                    });
                }
            }
        }

        private void SetLocalTransformation(ref Entity<Node, Fixture> node, in Body treeBody)
        {
            this._logger.Verbose("Preparing to set {LocalTransformation} for {Node} {Fixture} {NodeId}", nameof(Fixture.LocalTransform), nameof(Node), nameof(Fixture), node.LocalId);

            node.Second.SetBodyTransform(treeBody.Transform);

            if (!this._entityQueryService.TryQueryByGroupIndex<Coupling>(node.GroupIndex, out Coupling coupling) || coupling.SocketId == NodeSocketLocalId.Empty)
            {
                node.Second.SetLocalRotationTransform(Fix64.Zero, FixTransform2D.Identity);
                return;
            }

            try
            {
                ref Plug plug = ref this._entityQueryService.QueryByGroupIndex<Plug>(node.GroupIndex);
                NodeSocket nodeSocket = this._socketService.GetNodeSocket(coupling.SocketId);

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

                var localId = this._entityQueryService.QueryByGroupIndex<EntityLocalId>(node.GroupIndex);
                this._logger.Error(ex, "There was a fatal error attempting to set node transformation for node {NodeLocalId}.", localId);
                this._entitySpawnService.Despawn(NameSpace<NodeFixtureSystem>.Instance, localId);
#if DEBUG
                throw;
#endif
            }
        }
    }
}