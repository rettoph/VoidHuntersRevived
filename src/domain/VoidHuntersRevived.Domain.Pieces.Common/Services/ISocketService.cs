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
        NodeSocket GetSocket(NodeSocketId socketId);
        bool TryGetSocket(SocketVhId socketVhId, out NodeSocket nodeSocket);

        ref EntityFilterCollection GetCouplingFilter(NodeSocketId socketId);
        ref EntityFilterCollection GetCouplingFilter(EntityId nodeId, byte socketIndex);

        bool TryGetClosestOpenSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out NodeSocket nodeSocket);

        EntityId Spawn(VhId sourceId, NodeSocket targetSocketNode, VhId vhid, Key<IEntityTemplate> nodeTemplateKey, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, NodeSocket targetSocketNode, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
