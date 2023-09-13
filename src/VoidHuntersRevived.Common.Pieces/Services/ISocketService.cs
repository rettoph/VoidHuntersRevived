using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface ISocketService
    {
        Socket GetSocket(SocketId socketId);
        bool TryGetSocket(SocketVhId socketVhId, out Socket socketNode);

        ref EntityFilterCollection GetCouplingFilter(SocketId socketId);

        bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out Socket socket);

        EntityId Spawn(Socket socket, VhId nodeVhId, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(Socket socket, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
