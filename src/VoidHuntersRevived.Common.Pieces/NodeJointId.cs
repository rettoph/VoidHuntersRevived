using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct NodeJointId
    {
        public readonly VhId NodeId;
        public readonly byte Index;

        public NodeJointId(VhId nodeId, byte index)
        {
            NodeId = nodeId;
            Index = index;
        }
    }
}
