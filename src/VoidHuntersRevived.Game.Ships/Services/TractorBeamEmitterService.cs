using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
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
        private static readonly Fix64 OpenNodemaximumDistance = Fix64.One;

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
            if(!_entities.TryQueryById(tractorBeamEmitterId, out TractorBeamEmitter tractorBeamEmitter))
            {
                targetNode = default;
                return false;
            }

            AABB aabb = new AABB(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            Node? callbackTargetNode = default!;

            _space.QueryAABB(fixture =>
            {
                // BEGIN NODE DISTANCE CHECK
                EntityId queryNodeId = _entities.GetId(fixture.Id);
                ref Node queryNode = ref _entities.QueryById<Node>(queryNodeId, out GroupIndex nodeGroupIndex);
                ref Rigid queryRigid  = ref _entities.QueryByGroupIndex<Rigid>(nodeGroupIndex);

                FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Centeroid, queryNode.Transformation);
                FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                if (queryNodeDistance > minDistance)
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                if(_entities.TryQueryById(queryNode.TreeId, out Tractorable tractorable) && tractorable.TractorBeamEmitter == default)
                { // Target resides within a tractorable tree, so we want to grab the head
                    callbackTargetNode = _trees.GetHead(queryNode.TreeId);
                }
                else if(queryNode.TreeId == tractorBeamEmitterId && !_nodes.IsHead(queryNode))
                { // The node belongs to the current tractor beam emitter's ship and is not the head
                    callbackTargetNode = queryNode;
                }
                else
                { // Target is not in any way tractorable, we can disregard it
                    return true;
                }

                minDistance = queryNodeDistance;


                return true; // Ensure we only check a maximum of 5 fixtures all the way through

            }, ref aabb);

            if (callbackTargetNode is null)
            {
                targetNode = default;
                return false;
            }

            targetNode = callbackTargetNode.Value;
            return true;
        }

        public bool TryGetClosestOpenSocket(EntityId shipId, FixVector2 target, [MaybeNullWhen(false)] out SocketNode socketNode)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _entities.GetFilter<Node>(shipId, Tree.NodeFilterContextId);
            Fix64 closestOpenSocketDistance = OpenNodemaximumDistance;
            socketNode = default!;
            bool result = false;
        
            foreach (var (indeces, group) in filter)
            {
                if (!_entities.HasAny<Sockets>(group))
                {
                    continue;
                }

                var (nodes, socketses, _) = _entities.QueryEntities<Node, Sockets>(group);
        
                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    if (
                        this.TryGetClosestOpenSocketOnNode(target, ref nodes[index], ref socketses[index], out Fix64 closestOpenSocketOnNodeDistance, out var closestOpenSocketOnNode)
                        && closestOpenSocketOnNodeDistance < closestOpenSocketDistance)
                    {
                        closestOpenSocketDistance = closestOpenSocketOnNodeDistance;
                        socketNode = new SocketNode(ref closestOpenSocketOnNode.Socket, ref closestOpenSocketOnNode.Node);
                        result = true;
                    }
                }
            }
        
            return result;
        }
        
        private bool TryGetClosestOpenSocketOnNode(
            FixVector2 target,
            ref Node node,
            ref Sockets sockets,
            out Fix64 closestOpenSocketDistance,
            out SocketNode closestOpenSocketOnNode)
        {
            closestOpenSocketDistance = OpenNodemaximumDistance;
            closestOpenSocketOnNode = default!;
            bool result = false;
        
            for (int j = 0; j < sockets.Items.count; j++)
            {
                ref Socket socket = ref sockets.Items[j];

                var filter = _sockets.GetCouplingFilter(socket.Id);
                filter.ComputeFinalCount(out int count);
                if(count > 0)
                {
                    continue;
                }

                FixMatrix jointWorldTransformation = socket.Location.Transformation * node.Transformation;
                FixVector2 jointWorldPosition = FixVector2.Transform(FixVector2.Zero, jointWorldTransformation);
                FixVector2.Distance(ref jointWorldPosition, ref target, out Fix64 jointDistanceFromTarget);
                if (jointDistanceFromTarget > closestOpenSocketDistance)
                { // Socket is further away than previously checked closest
                    continue;
                }
        
                closestOpenSocketDistance = jointDistanceFromTarget;
                closestOpenSocketOnNode = new SocketNode(ref socket, ref node);
                result = true;
            }
        
            return result;
        }
    }
}
