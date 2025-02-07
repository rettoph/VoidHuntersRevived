using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Logging.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Extensions.Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed partial class NodeSocketService(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        IEntitySerializationService entitySerializationService,
        ILogger logger
    ) : INodeSocketService
    {
        private static readonly Fix64 _openNodeMaximumDistance = Fix64.One;

        private readonly ILogger _logger = logger;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly IEntitySerializationService _entitySerializationService = entitySerializationService;

        public NodeSocketGlobalId GetGlobalId(NodeSocketLocalId nodeSocketLocalId)
        {
            return new(
                nodeGlobalId: this._entityQueryService.GetGlobalId(nodeSocketLocalId.NodeLocalId),
                socketIndex: nodeSocketLocalId.SocketIndex);
        }

        public NodeSocketLocalId GetLocalId(NodeSocketGlobalId nodeSocketGlobalId)
        {
            return new(
                nodeLocalId: this._entityQueryService.GetLocalId(nodeSocketGlobalId.NodeGlobalId),
                socketIndex: nodeSocketGlobalId.SocketIndex);
        }

        public NodeSocket GetNodeSocket(NodeSocketLocalId nodeSocketLocalId)
        {
            this._logger.Verbose("GetNodeSocket - NodeSocketLocalId = {NodeSocketLocalId}", nodeSocketLocalId);

            ref Node node = ref this._entityQueryService.QueryByLocalId<Node>(nodeSocketLocalId.NodeLocalId, out GroupIndex groupIndex);
            var (fixtures, sockets, _) = this._entityQueryService.QueryEntities<Fixture, Sockets>(groupIndex.GroupID);
            NodeSocket nodeSocket = new(
                bodyLocalId: fixtures[groupIndex.Index].BodyFilterId.EGID.ToEntityLocalId(),
                localId: nodeSocketLocalId,
                node: node,
                fixture: fixtures[groupIndex.Index],
                socket: sockets[groupIndex.Index].Items[nodeSocketLocalId.SocketIndex]);

            return nodeSocket;
        }

        public bool TryGetNodeSocket(NodeSocketGlobalId nodeSocketGlobalId, out NodeSocket nodeSocket)
        {
            if (this._entityQueryService.TryGetLocalId(nodeSocketGlobalId.NodeGlobalId, out EntityLocalId nodeLocalId))
            {
                nodeSocket = this.GetNodeSocket(new NodeSocketLocalId(nodeLocalId, nodeSocketGlobalId.SocketIndex));
                return true;
            }

            nodeSocket = default;
            return false;
        }

        public bool TryGetNodeSocket(NodeSocketLocalId nodeSocketLocalId, out NodeSocket nodeSocket)
        {
            this._logger.Verbose("TryGetNodeSocket - NodeSocketLocalId = {NodeSocketLocalId}", nodeSocketLocalId);

            if (this._entityQueryService.TryQueryByLocalId<Node>(nodeSocketLocalId.NodeLocalId, out GroupIndex groupIndex, out Node node) == false)
            {
                nodeSocket = default;
                return false;
            }

            var (fixtures, sockets, _) = this._entityQueryService.QueryEntities<Fixture, Sockets>(groupIndex.GroupID);
            nodeSocket = new(
                bodyLocalId: fixtures[groupIndex.Index].BodyFilterId.EGID.ToEntityLocalId(),
                localId: nodeSocketLocalId,
                node: node,
                fixture: fixtures[groupIndex.Index],
                socket: sockets[groupIndex.Index].Items[nodeSocketLocalId.SocketIndex]);

            return true;
        }

        public ref EntityFilterCollection GetCouplingFilter(NodeSocketLocalId socketId)
        {
            return ref this._entityQueryService.GetFilter<Coupling>(socketId.NodeLocalId, socketId.FilterContextId);
        }

        public ref EntityFilterCollection GetCouplingFilter(EntityLocalId nodeLocalId, byte socketIndex)
        {
            return ref this.GetCouplingFilter(new NodeSocketLocalId(nodeLocalId, socketIndex));
        }

        public bool TryGetClosestOpenNodeSocket(EntityLocalId treeLocalId, FixVector2 worldPosition, [MaybeNullWhen(false)] out NodeSocket nodeSocket)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref this._entityQueryService.GetCompositeFilter<Body, Fixture, Sockets>(treeLocalId);
            Fix64 closestOpenSocketDistance = _openNodeMaximumDistance;
            nodeSocket = default!;
            bool result = false;

            foreach (var (indeces, group) in filter)
            {
                var (statuses, nodes, fixtures, sockets, _) = this._entityQueryService.QueryEntities<EntityStatus, Node, Fixture, Sockets>(group);

                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    NodeSockets nodeSockets = new(fixtures[index].BodyFilterId.EGID.ToEntityLocalId(), index, nodes, fixtures, sockets);
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
            closestOpenSocketDistance = _openNodeMaximumDistance;
            closestOpenSocketOnNode = default!;
            bool result = false;

            for (byte j = 0; j < nodeSockets.Count; j++)
            {
                NodeSocket nodeSocket = nodeSockets[j];

                var filter = this.GetCouplingFilter(nodeSockets.Node.LocalId, j);
                int count = 0;
                foreach (var (indices, groupId) in filter)
                {
                    var (entityStatuses, _) = this._entityQueryService.QueryEntities<EntityStatus>(groupId);

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

                FixVector2 socketWorldPosition = FixVector2.Transform(FixVector2.Zero, nodeSocket.WorldTransform);
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