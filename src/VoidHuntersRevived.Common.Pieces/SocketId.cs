using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct SocketId
    {
        public readonly EntityId NodeId;
        public readonly byte Index;

        public SocketVhId VhId => new SocketVhId(NodeId.VhId, Index);

        public SocketId(EntityId nodeId, byte index)
        {
            NodeId = nodeId;
            Index = index;
        }
    }
}
