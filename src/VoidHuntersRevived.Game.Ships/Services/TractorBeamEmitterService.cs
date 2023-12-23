using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Ships.Services
{
    public sealed partial class TractorBeamEmitterService : BasicEngine, ITractorBeamEmitterService
    {
        private static Fix64 QueryRadius = (Fix64)3;

        private readonly ISpace _space;
        private readonly IEntityService _entities;
        private readonly INodeService _nodes;
        private readonly ITreeService _trees;
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public TractorBeamEmitterService(ISpace space, IEntityService entities, INodeService nodes, ITreeService trees, ISocketService sockets, ILogger logger)
        {
            _space = space;
            _entities = entities;
            _nodes = nodes;
            _trees = trees;
            _sockets = sockets;
            _logger = logger;
        }

        public ref EntityFilterCollection GetTractorableFilter(EntityId tractorBeamEmitterId)
        {
            return ref _entities.GetFilter<Tractorable>(tractorBeamEmitterId, TractorBeamEmitter.TractorableFilterContext);
        }

        public bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out Node targetNode)
        {
            if (!_entities.TryQueryById(tractorBeamEmitterId, out TractorBeamEmitter tractorBeamEmitter))
            {
                targetNode = default;
                return false;
            }

            AABB aabb = new AABB(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            Node? callbackTargetNode = default!;

            _space.QueryAABB(fixture =>
            {
                if (_entities.IsSpawned(fixture.EntityId))
                {
                    ref Node queryNode = ref _entities.QueryById<Node>(fixture.EntityId, out GroupIndex nodeGroupIndex);
                    ref Rigid queryRigid = ref _entities.QueryByGroupIndex<Rigid>(nodeGroupIndex);

                    FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Centeroid, queryNode.Transformation);
                    FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                    if (queryNodeDistance > minDistance)
                    { // Invalid Target - The distance is further away than the previously closest valid target
                        return true;
                    }

                    ref Tree tree = ref _entities.QueryById<Tree>(queryNode.TreeId, out GroupIndex treeGroupIndex);
                    if (_entities.TryQueryByGroupIndex(treeGroupIndex, out Tractorable tractorable) && tractorable.TractorBeamEmitter == default)
                    { // Target resides within a tractorable tree, so we want to grab the head
                        callbackTargetNode = tree.HeadId == queryNode.Id ? queryNode : _entities.QueryById<Node>(tree.HeadId);
                    }
                    else if (queryNode.TreeId == tractorBeamEmitterId && tree.HeadId != queryNode.Id)
                    { // The node belongs to the current tractor beam emitter's ship and is not the head
                        callbackTargetNode = queryNode;
                    }
                    else
                    { // Target is not in any way tractorable, we can disregard it
                        return true;
                    }

                    if (!_entities.IsSpawned(treeGroupIndex))
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
