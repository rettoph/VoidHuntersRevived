using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Factories
{
    public interface ITreeFactory
    {
        void Create(VhId vhid, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> piece);
        void Create(VhId vhid, IEntityType<TreeDescriptor> tree, EntityData nodes);
    }
}
