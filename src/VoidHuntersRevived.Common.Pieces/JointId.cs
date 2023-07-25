using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct JointId
    {
        public readonly EntityId NodeId;
        public readonly byte Index;

        public JointVhId VhId => new JointVhId(NodeId.VhId, Index);

        public JointId(EntityId nodeId, byte index)
        {
            NodeId = nodeId;
            Index = index;
        }
    }
}
