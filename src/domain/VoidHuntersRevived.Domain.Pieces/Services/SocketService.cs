using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed partial class SocketService(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        IEntitySerializationService entitySerializationService,
        ITreeService treeService,
        ILogger logger) : StrategyEngine, ISocketService
    {
        private static readonly Fix64 OpenNodemaximumDistance = Fix64.One;

        private readonly ILogger _logger = logger;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService = entitySerializationService;
        private readonly ITreeService _treeService = treeService;

        public NodeSocket GetSocket(NodeSocketId socketId)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Locating {NodeId}:{SocketIndex} - Node EGID {EntityId}:{GroupId}", nameof(SocketService), nameof(GetSocket), socketId.NodeId.VhId.Value, socketId.Index, socketId.NodeId.EGID.entityID, socketId.NodeId.EGID.groupID);

            ref Node node = ref _entityQueryService.QueryById<Node>(socketId.NodeId, out GroupIndex groupIndex);
            var (sockets, _) = _entityQueryService.QueryEntities<Sockets>(groupIndex.GroupID);

            NodeSocket nodeSocket = new(node, socketId, sockets[groupIndex.Index].Items[socketId.Index]);

            return nodeSocket;
        }

        public bool TryGetSocket(SocketVhId socketVhId, out NodeSocket nodeSocket)
        {
            if (_entityQueryService.TryGetId(socketVhId.NodeVhId, out EntityId nodeId))
            {
                nodeSocket = this.GetSocket(new NodeSocketId(nodeId, socketVhId.Index));
                return true;
            }

            nodeSocket = default;
            return false;
        }

        public ref EntityFilterCollection GetCouplingFilter(NodeSocketId socketId)
        {
            return ref _entityQueryService.GetFilter<Coupling>(socketId.NodeId, socketId.FilterContextId);
        }

        public ref EntityFilterCollection GetCouplingFilter(EntityId nodeId, byte socketIndex)
        {
            return ref this.GetCouplingFilter(new NodeSocketId(nodeId, socketIndex));
        }

        public bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out NodeSocket nodeSocket)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _entityQueryService.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
            Fix64 closestOpenSocketDistance = OpenNodemaximumDistance;
            nodeSocket = default!;
            bool result = false;

            foreach (var (indeces, group) in filter)
            {
                if (!_entityQueryService.HasAny<Sockets>(group))
                {
                    continue;
                }

                var (statuses, nodes, sockets, _) = _entityQueryService.QueryEntities<EntityStatus, Node, Sockets>(group);

                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    NodeSockets nodeSockets = new(index, nodes, sockets);
                    if (statuses[index].IsSpawned
                        && this.TryGetClosestOpenSocketOnNode(worldPosition, ref nodeSockets, out Fix64 closestOpenSocketOnNodeDistance, out NodeSocket closestOpenSocketOnNode)
                        && closestOpenSocketOnNodeDistance < closestOpenSocketDistance)
                    {
                        closestOpenSocketDistance = closestOpenSocketOnNodeDistance;
                        nodeSocket = closestOpenSocketOnNode;
                        result = true;
                    }
                }
            }

            return result;
        }

        private bool TryGetClosestOpenSocketOnNode(
            FixVector2 worldPosition,
            ref NodeSockets nodeSockets,
            out Fix64 closestOpenSocketDistance,
            out NodeSocket closestOpenSocketOnNode)
        {
            closestOpenSocketDistance = OpenNodemaximumDistance;
            closestOpenSocketOnNode = default!;
            bool result = false;

            for (byte j = 0; j < nodeSockets.Count; j++)
            {
                NodeSocket nodeSocket = nodeSockets[j];

                var filter = this.GetCouplingFilter(nodeSockets.Node.Id, j);
                int count = 0;
                foreach (var (indices, groupId) in filter)
                {
                    var (entityStatuses, _) = _entityQueryService.QueryEntities<EntityStatus>(groupId);

                    for (int i = 0; i < indices.count; i++)
                    {
                        if (entityStatuses[indices[i]].IsSpawned)
                        {
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    continue;
                }

                FixVector2 socketWorldPosition = FixVector2.Transform(FixVector2.Zero, nodeSocket.Transformation);
                FixVector2.Distance(ref socketWorldPosition, ref worldPosition, out Fix64 jointDistanceFromTarget);
                if (jointDistanceFromTarget > closestOpenSocketDistance)
                { // Socket is further away than previously checked closest
                    continue;
                }

                closestOpenSocketDistance = jointDistanceFromTarget;
                closestOpenSocketOnNode = nodeSocket;
                result = true;
            }

            return result;
        }
    }
}
