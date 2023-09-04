using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Exceptions;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : BasicEngine, ISocketService
    {
        private static readonly Fix64 OpenNodemaximumDistance = Fix64.One;

        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly ITreeService _trees;

        public SocketService(IEntityService entities, ITreeService trees, ILogger logger)
        {
            _logger = logger;
            _entities = entities;
            _trees = trees;
        }

        public SocketNode GetSocketNode(SocketId socketId)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Locating {NodeId}:{SocketIndex} - Node EGID {EntityId}:{GroupId}", nameof(SocketService), nameof(GetSocketNode), socketId.NodeId.VhId.Value, socketId.Index, socketId.NodeId.EGID.entityID, socketId.NodeId.EGID.groupID);

            ref Node node = ref _entities.QueryById<Node>(socketId.NodeId, out GroupIndex groupIndex);
            ref Sockets sockets = ref _entities.QueryByGroupIndex<Sockets>(groupIndex);


            return new SocketNode(ref sockets.Items[socketId.Index], ref node);
        }

        public bool TryGetSocketNode(SocketVhId socketVhId, out SocketNode socketNode)
        {
            if(_entities.TryGetId(socketVhId.NodeVhId, out EntityId nodeId))
            {
                socketNode = this.GetSocketNode(new SocketId(nodeId, socketVhId.Index));
                return true;
            }

            socketNode = default;
            return false;
        }

        public ref EntityFilterCollection GetCouplingFilter(SocketId socketId)
        {
            return ref _entities.GetFilter<Coupling>(socketId.NodeId, socketId.FilterContextId);
        }

        public bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out SocketNode socketNode)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
            Fix64 closestOpenSocketDistance = OpenNodemaximumDistance;
            socketNode = default!;
            bool result = false;

            foreach (var (indeces, group) in filter)
            {
                if (!_entities.HasAny<Sockets>(group))
                {
                    continue;
                }

                var (statuses, nodes, socketses, _) = _entities.QueryEntities<EntityStatus, Node, Sockets>(group);

                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    if (statuses[index].IsSpawned
                        && this.TryGetClosestOpenSocketOnNode(worldPosition, ref nodes[index], ref socketses[index], out Fix64 closestOpenSocketOnNodeDistance, out var closestOpenSocketOnNode)
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
            FixVector2 worldPosition,
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

                var filter = this.GetCouplingFilter(socket.Id);
                int count = 0;
                foreach (var (indices, groupId) in filter)
                {
                    var (entityStatuses, _) = _entities.QueryEntities<EntityStatus>(groupId);

                    for (int i = 0; i < indices.count; i++)
                    {
                        if (entityStatuses[indices[i]].IsSpawned)
                        {
                            count++;
                        }
                    }
                }

                if(count > 0)
                {
                    continue;
                }

                FixMatrix jointWorldTransformation = socket.Location.Transformation * node.Transformation;
                FixVector2 jointWorldPosition = FixVector2.Transform(FixVector2.Zero, jointWorldTransformation);
                FixVector2.Distance(ref jointWorldPosition, ref worldPosition, out Fix64 jointDistanceFromTarget);
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
