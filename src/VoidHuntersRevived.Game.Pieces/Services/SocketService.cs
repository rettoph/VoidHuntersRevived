using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Exceptions;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed partial class SocketService : BasicEngine, ISocketService
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

        public Socket GetSocket(SocketId socketId)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Locating {NodeId}:{SocketIndex} - Node EGID {EntityId}:{GroupId}", nameof(SocketService), nameof(GetSocket), socketId.NodeId.VhId.Value, socketId.Index, socketId.NodeId.EGID.entityID, socketId.NodeId.EGID.groupID);

            ref Node node = ref _entities.QueryById<Node>(socketId.NodeId, out GroupIndex groupIndex);
            var (socketIds, socketLocations, _) = _entities.QueryEntities<Sockets<SocketId>, Sockets<Location>>(groupIndex.GroupID);

            Socket socket = new Socket(node, socketIds[groupIndex.Index].Items[socketId.Index], socketLocations[groupIndex.Index].Items[socketId.Index]);

            return socket;
        }

        public bool TryGetSocket(SocketVhId socketVhId, out Socket socket)
        {
            if(_entities.TryGetId(socketVhId.NodeVhId, out EntityId nodeId))
            {
                socket = this.GetSocket(new SocketId(nodeId, socketVhId.Index));
                return true;
            }

            socket = default;
            return false;
        }

        public ref EntityFilterCollection GetCouplingFilter(SocketId socketId)
        {
            return ref _entities.GetFilter<Coupling>(socketId.NodeId, socketId.FilterContextId);
        }

        public bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out Socket socket)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
            Fix64 closestOpenSocketDistance = OpenNodemaximumDistance;
            socket = default!;
            bool result = false;

            foreach (var (indeces, group) in filter)
            {
                if (!_entities.HasAny<Sockets<Location>>(group))
                {
                    continue;
                }

                var (statuses, nodes, socketIds, socketLocations, _) = _entities.QueryEntities<EntityStatus, Node, Sockets<SocketId>, Sockets<Location>>(group);

                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    Sockets sockets = new Sockets(index, nodes, socketIds, socketLocations);
                    if (statuses[index].IsSpawned
                        && this.TryGetClosestOpenSocketOnNode(worldPosition, ref sockets, out Fix64 closestOpenSocketOnNodeDistance, out Socket closestOpenSocketOnNode)
                        && closestOpenSocketOnNodeDistance < closestOpenSocketDistance)
                    {
                        closestOpenSocketDistance = closestOpenSocketOnNodeDistance;
                        socket = closestOpenSocketOnNode;
                        result = true;
                    }
                }
            }

            return result;
        }

        private bool TryGetClosestOpenSocketOnNode(
            FixVector2 worldPosition,
            ref Sockets sockets,
            out Fix64 closestOpenSocketDistance,
            out Socket closestOpenSocketOnNode)
        {
            closestOpenSocketDistance = OpenNodemaximumDistance;
            closestOpenSocketOnNode = default!;
            bool result = false;

            for (byte j = 0; j < sockets.Count; j++)
            {
                Socket socket = sockets[j];

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

                FixVector2 socketWorldPosition = FixVector2.Transform(FixVector2.Zero, socket.Transformation);
                FixVector2.Distance(ref socketWorldPosition, ref worldPosition, out Fix64 jointDistanceFromTarget);
                if (jointDistanceFromTarget > closestOpenSocketDistance)
                { // Socket is further away than previously checked closest
                    continue;
                }

                closestOpenSocketDistance = jointDistanceFromTarget;
                closestOpenSocketOnNode = socket;
                result = true;
            }

            return result;
        }
    }
}
