using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct SocketVhId
    {
        public readonly VhId NodeVhId;
        public readonly byte Index;

        public SocketVhId(VhId nodeVhId, byte index)
        {
            NodeVhId = nodeVhId;
            Index = index;
        }

        public static bool operator ==(SocketVhId socketVhId1, SocketVhId socketVhId2)
        {
            return socketVhId1.NodeVhId == socketVhId2.NodeVhId && socketVhId1.Index == socketVhId2.Index;
        }

        public static bool operator !=(SocketVhId socketVhId1, SocketVhId socketVhId2)
        {
            return socketVhId1.NodeVhId != socketVhId2.NodeVhId || socketVhId1.Index != socketVhId2.Index;
        }
    }
}
