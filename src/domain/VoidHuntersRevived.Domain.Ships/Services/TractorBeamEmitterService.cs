using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
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
        ISocketService socketService,
        ITeamService teamService,
        ILogger logger) : StrategyEngine, ITractorBeamEmitterService
    {
        private static readonly Fix64 QueryRadius = (Fix64)3;

        private readonly ISpace _space = space;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService = entitySerializationService;
        private readonly INodeService _nodeService = nodeService;
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly ISocketService _socketService = socketService;
        private readonly ILogger _logger = logger;

        public ref EntityFilterCollection GetTractorableFilter(EntityLocalId tractorBeamEmitterLocalId)
        {
            return ref _entityQueryService.GetFilter<Tractorable, TractorBeamEmitter>(tractorBeamEmitterLocalId);
        }

        public bool Query(EntityGlobalId tractorBeamEmitterGlobalId, FixVector2 target, out Node targetNode)
        {
            AABB aabb = new(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            Node? callbackTargetNode = default!;

            _space.QueryAABB(fixture =>
            {
                if (_entityQueryService.IsSpawned(fixture.Id.EntityLocalId))
                {
                    ref Node queryNode = ref _entityQueryService.QueryByLocalId<Node>(fixture.Id.EntityLocalId, out GroupIndex nodeGroupIndex);
                    ref Rigid queryRigid = ref _entityQueryService.QueryByGroupIndex<Rigid>(nodeGroupIndex);

                    FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Template.Value.Centeroid, queryNode.Transformation);
                    FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                    if (queryNodeDistance > minDistance)
                    { // Invalid Target - The distance is further away than the previously closest valid target
                        return true;
                    }

                    ref Tree tree = ref _entityQueryService.QueryById<Tree>(queryNode.TreeId, out GroupIndex treeGroupIndex);
                    if (_entityQueryService.TryQueryByGroupIndex(treeGroupIndex, out Tractorable tractorable) && tractorable.TractorBeamEmitterLocalId == default)
                    { // Target resides within a tractorable tree, so we want to grab the head
                        callbackTargetNode = tree.HeadId == queryNode.Id ? queryNode : _entityQueryService.QueryById<Node>(tree.HeadId);
                    }
                    else if (queryNode.TreeId.ToGlobalEntityId() == tractorBeamEmitterGlobalId && tree.HeadId != queryNode.Id)
                    { // The node belongs to the current tractor beam emitter's ship and is not the head
                        callbackTargetNode = queryNode;
                    }
                    else
                    { // Target is not in any way tractorable, we can disregard it
                        return true;
                    }

                    if (!_entityQueryService.IsSpawned(treeGroupIndex))
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
