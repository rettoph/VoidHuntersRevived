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
    }
}
