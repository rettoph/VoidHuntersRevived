using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface ITreeService
    {
        ref Node GetHead(in Tree tree);
        ref Node GetHead(in EntityId treeId);
        ref Node GetHead(in GroupIndex treeGroupIndex);

        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializer);
        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, Blueprint blueprint, EntityInitializerDelegate? initializer = null);
    }
}
