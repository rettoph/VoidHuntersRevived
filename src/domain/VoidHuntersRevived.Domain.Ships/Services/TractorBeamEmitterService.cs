using Serilog;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public sealed partial class TractorBeamEmitterService(
        ISpace space,
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        IEntitySerializationService entitySerializationService,
        INodeService nodeService,
        ITreeService treeService,
        INodeSocketService socketService,
        ITeamService teamService,
        ILogger logger) : StrategyEngine, ITractorBeamEmitterService
    {
        private static readonly Fix64 _queryRadius = (Fix64)3;

        private readonly ISpace _space = space;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService = entitySerializationService;
        private readonly INodeService _nodeService = nodeService;
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly INodeSocketService _socketService = socketService;
        private readonly ILogger _logger = logger;

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