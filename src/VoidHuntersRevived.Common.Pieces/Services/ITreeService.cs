using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        EntityId Spawn(VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> node, EntityInitializerDelegate? initializer = null);
        EntityId Spawn(VhId vhid, Id<ITeam> teamId, IEntityType<TreeDescriptor> tree, EntityData nodes, EntityInitializerDelegate initializer);
    }
}
