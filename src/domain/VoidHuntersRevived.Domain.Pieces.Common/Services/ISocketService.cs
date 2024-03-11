using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface ISocketService
    {
        Socket GetSocket(SocketId socketId);
        bool TryGetSocket(SocketVhId socketVhId, out Socket socketNode);

        ref EntityFilterCollection GetCouplingFilter(SocketId socketId);

        bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out Socket socket);

        EntityId Spawn(VhId sourceId, Socket socket, VhId nodeVhId, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, Socket socket, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
