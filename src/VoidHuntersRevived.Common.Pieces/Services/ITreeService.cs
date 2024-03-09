using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface ITreeService
    {
        ref Node GetHead(in Tree tree);
        ref Node GetHead(in EntityId treeId);
        ref Node GetHead(in GroupIndex treeGroupIndex);

        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> node, InstanceEntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, EntityData nodes, InstanceEntityInitializerDelegate initializer);
        EntityId Spawn(VhId sourceId, VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, Blueprint blueprint, InstanceEntityInitializerDelegate? initializer = null);
    }
}
