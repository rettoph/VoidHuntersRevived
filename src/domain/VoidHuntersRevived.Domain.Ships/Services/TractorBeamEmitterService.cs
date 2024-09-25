using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public sealed partial class TractorBeamEmitterService : StrategyEngine, ITractorBeamEmitterService
    {
        private static Fix64 QueryRadius = (Fix64)3;

        private readonly ISpace _space;
        private readonly IEntityQueryService _entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService;
        private readonly INodeService _nodeService;
        private readonly ITreeService _treeService;
        private readonly ITeamService _teamService;
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public TractorBeamEmitterService(
            ISpace space,
            IEntityQueryService entityQueryService,
            IEntitySpawnService entitySpawnService,
            IEntitySerializationService entitySerializationService,
            INodeService nodeService,
            ITreeService treeService,
            ISocketService socketService,
            ITeamService teamService,
            ILogger logger)
        {
            _space = space;
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
            _entitySerializationService = entitySerializationService;
            _nodeService = nodeService;
            _treeService = treeService;
            _socketService = socketService;
            _teamService = teamService;
            _logger = logger;
        }

        public ref EntityFilterCollection GetTractorableFilter(EntityId tractorBeamEmitterId)
        {
            return ref _entityQueryService.GetFilter<Tractorable>(tractorBeamEmitterId, TractorBeamEmitter.TractorableFilterContext);
        }

        public bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out Node targetNode)
        {
            if (!_entityQueryService.TryQueryById(tractorBeamEmitterId, out TractorBeamEmitter tractorBeamEmitter))
            {
                targetNode = default;
                return false;
            }

            AABB aabb = new AABB(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            Node? callbackTargetNode = default!;

            _space.QueryAABB(fixture =>
            {
                if (_entityQueryService.IsSpawned(fixture.EntityId))
                {
                    ref Node queryNode = ref _entityQueryService.QueryById<Node>(fixture.EntityId, out GroupIndex nodeGroupIndex);
                    ref Rigid queryRigid = ref _entityQueryService.QueryByGroupIndex<Rigid>(nodeGroupIndex);

                    FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Centeroid, queryNode.Transformation);
                    FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                    if (queryNodeDistance > minDistance)
                    { // Invalid Target - The distance is further away than the previously closest valid target
                        return true;
                    }

                    ref Tree tree = ref _entityQueryService.QueryById<Tree>(queryNode.TreeId, out GroupIndex treeGroupIndex);
                    if (_entityQueryService.TryQueryByGroupIndex(treeGroupIndex, out Tractorable tractorable) && tractorable.TractorBeamEmitter == default)
                    { // Target resides within a tractorable tree, so we want to grab the head
                        callbackTargetNode = tree.HeadId == queryNode.Id ? queryNode : _entityQueryService.QueryById<Node>(tree.HeadId);
                    }
                    else if (queryNode.TreeId == tractorBeamEmitterId && tree.HeadId != queryNode.Id)
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
