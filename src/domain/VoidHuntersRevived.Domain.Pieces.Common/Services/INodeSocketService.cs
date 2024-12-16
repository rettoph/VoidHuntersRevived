using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface INodeSocketService
    {
        NodeSocketGlobalId GetGlobalId(NodeSocketLocalId nodeSocketLocalId);
        NodeSocketLocalId GetLocalId(NodeSocketGlobalId nodeSockeGlobalId);

        NodeSocket GetNodeSocket(NodeSocketLocalId nodeSocketLocalId);

        bool TryGetNodeSocket(NodeSocketGlobalId nodeSocketGlobalId, out NodeSocket nodeSocket);
        bool TryGetNodeSocket(NodeSocketLocalId nodeSocketLocalId, out NodeSocket nodeSocket);

        ref EntityFilterCollection GetCouplingFilter(NodeSocketLocalId socketId);
        ref EntityFilterCollection GetCouplingFilter(EntityLocalId nodeLocalId, byte socketIndex);

        bool TryGetClosestOpenNodeSocket(EntityId treeId, FixVector2 worldPosition, [MaybeNullWhen(false)] out NodeSocket nodeSocket);

        EntityId Spawn(VhId sourceId, NodeSocket targetSocketNode, EntityGlobalId globalId, Key<IEntityTemplate> nodeTemplateKey, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, NodeSocket targetSocketNode, EntityData nodes, EntityInitializerDelegate? initializer = null);
    }
}
