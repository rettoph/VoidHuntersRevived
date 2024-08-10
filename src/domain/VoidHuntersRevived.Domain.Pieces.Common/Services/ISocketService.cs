using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface ISocketService
    {
        Socket GetSocket(SocketId socketId);
        bool TryGetSocket(SocketVhId socketVhId, out Socket socketNode);

        ref EntityFilterCollection GetCouplingFilter(SocketId socketId);

        bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out Socket socket);

        EntityId Spawn(VhId sourceId, Socket socket, VhId nodeVhId, IKey<IEntityType> nodeTypeKey, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, Socket socket, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
