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
        SocketNode GetSocketNode(SocketId socketId);
        bool TryGetSocketNode(SocketVhId socketVhId, out SocketNode socketNode);

        ref EntityFilterCollection GetCouplingFilter(SocketId socketId);

        bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out SocketNode socketNode);

        EntityId Spawn(SocketNode socketNode, VhId nodeVhId, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(SocketNode socketNode, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
