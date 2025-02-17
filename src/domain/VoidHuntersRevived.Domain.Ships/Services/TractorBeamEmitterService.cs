using Guppy.Core.Logging.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public partial class TractorBeamEmitterService(
        IStrategy strategy,
        ISpace space,
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        IEntitySerializationService entitySerializationService,
        INodeService nodeService,
        ITreeService treeService,
        INodeSocketService socketService,
        ITeamService teamService,
        ILogger logger
    ) : ITractorBeamEmitterService
    {
        private static readonly Fix64 _queryRadius = (Fix64)3;

        private readonly IStrategy _strategy = strategy;
        private readonly ISpace _space = space;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService = entitySerializationService;
        private readonly INodeService _nodeService = nodeService;
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly INodeSocketService _socketService = socketService;
        private readonly ILogger _logger = logger;

        public void Select(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, EntityGlobalId nodeGlobalId)
        {
            if (this._entityQueryService.IsSpawned(nodeGlobalId, out GroupIndex nodeGroupIndex) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} does not exist", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.TryQueryByGroupIndex<Node>(nodeGroupIndex, out Node node) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} is not a valid Node", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.TryQueryByGroupIndex<Fixture>(nodeGroupIndex, out Fixture fixture) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} is not a valid Fixture", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.IsSpawned(node.TreeLocalId) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} Tree {TreeLocalId} does not exist", nodeGlobalId, node.TreeLocalId);
                return;
            }

            this._logger.Verbose("Selecting {NodeGlobalId} with TractorBeamEmitter {TractorBeamEmitterGlobalId}", nodeGlobalId, tractorBeamEmitterGlobalId);
            this._strategy.Events.Publish(NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId), new TractorBeamEmitter_Select()
            {
                TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                TargetData = this._entitySerializationService.Serialize(nodeGroupIndex.GroupID, nodeGroupIndex.Index, SerializationOptions.Default),
                Transform = fixture.WorldTransform
            });


            if (this._nodeService.IsHead(in node))
            {
                this._logger.Verbose("Despawning Node {NodeGlobalId} Tree {TreeLocalId}", nodeGlobalId, node.TreeLocalId);
                this._entitySpawnService.Despawn(sourceId, node.TreeLocalId);
            }
            else
            {
                this._logger.Verbose("Despawning Node {NodeGlobalId}", nodeGlobalId);
                this._entitySpawnService.Despawn(sourceId, nodeGlobalId);
            }
        }

        private readonly Queue<(EntityLocalId localId, EntityLocalId headLocalId, Body body)> _deselecteds = new();
        public void Deselect(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, NodeSocketGlobalId? attachToSocketVhId)
        {
            if (this._entityQueryService.TryGetLocalId(tractorBeamEmitterGlobalId, out EntityLocalId tracorBeamEmitterLocalId) == false)
            {
                throw new NotImplementedException();
            }

            ref var filter = ref this._entityQueryService.GetFilter<TractorBeamEmitter, Tractorable>(tracorBeamEmitterLocalId);
            foreach (var (indices, groupId) in filter)
            {
                var (localIds, statuses, trees, transforms, _) = this._entityQueryService.QueryEntities<EntityLocalId, EntityStatus, Tree, Body>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    EntityLocalId localId = localIds[index];

                    if (statuses[index].IsDespawned)
                    {
                        this._logger.Warning("Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", localId);
                        continue;
                    }


                    this._deselecteds.Enqueue((localId, trees[index].HeadLocalId, transforms[index]));

                    filter.Remove(localId);
                }
            }

            VhId nextSourceId = NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId);
            while (this._deselecteds.TryDequeue(out (EntityLocalId localId, EntityLocalId headLocalId, Body body) deselected))
            {
                this._logger.Verbose("Attempting to deselect {TreeId} with emitter {TractorBeamEmitterLocalId}", deselected.localId, tractorBeamEmitterGlobalId);
                this._strategy.Events.Publish(nextSourceId, new TractorBeamEmitter_Deselect()
                {
                    TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                    TargetData = this._entitySerializationService.Serialize(deselected.headLocalId, SerializationOptions.Default),
                    Transform = deselected.body.Transform,
                    AttachToSocketVhId = attachToSocketVhId
                });
                this._entitySpawnService.Despawn(nextSourceId, deselected.localId);
            }
        }

        public bool Query(EntityLocalId tractorBeamEmitterLocalId, FixVector2 target, out Node targetNode)
        {
            AABB aabb = new(target, _queryRadius, _queryRadius);
            Fix64 minDistance = _queryRadius;
            Node? callbackTargetNode = default!;

            this._space.QueryAABB(fixture =>
            {
                if (this._entityQueryService.IsSpawned(fixture.Id.EntityLocalId))
                {
                    ref Node queryNode = ref this._entityQueryService.QueryByLocalId<Node>(fixture.Id.EntityLocalId, out GroupIndex nodeGroupIndex);
                    ref Rigid queryRigid = ref this._entityQueryService.QueryByGroupIndex<Rigid>(nodeGroupIndex);
                    ref Fixture queryFixture = ref this._entityQueryService.QueryByGroupIndex<Fixture>(nodeGroupIndex);

                    FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Template.Value.Centeroid, queryFixture.WorldTransform);
                    FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                    if (queryNodeDistance > minDistance)
                    { // Invalid Target - The distance is further away than the previously closest valid target
                        return true;
                    }

                    ref Tree tree = ref this._entityQueryService.QueryByLocalId<Tree>(queryNode.TreeLocalId, out GroupIndex treeGroupIndex);
                    if (this._entityQueryService.TryQueryByGroupIndex(treeGroupIndex, out Tractorable tractorable) && tractorable.TractorBeamEmitterFilterId.IsDefault<TractorBeamEmitter>())
                    { // Target resides within a tractorable tree, so we want to grab the head
                        callbackTargetNode = tree.HeadLocalId == queryNode.LocalId ? queryNode : this._entityQueryService.QueryByLocalId<Node>(tree.HeadLocalId);
                    }
                    else if (queryNode.TreeLocalId == tractorBeamEmitterLocalId && tree.HeadLocalId != queryNode.LocalId)
                    { // The node belongs to the current tractor beam emitter's ship and is not the head
                        callbackTargetNode = queryNode;
                    }
                    else
                    { // Target is not in any way tractorable, we can disregard it
                        return true;
                    }

                    if (!this._entityQueryService.IsSpawned(treeGroupIndex))
                    { // Tree has been soft despawned
                        return true;
                    }

                    minDistance = queryNodeDistance;


                    return true; // Ensure we only check a maximum of 5 fixtures all the way through
                }

                return false;
            }, ref aabb);

            if (callbackTargetNode is null)
            {
                targetNode = default;
                return false;
            }

            targetNode = callbackTargetNode.Value;
            return true;
        }
    }
}