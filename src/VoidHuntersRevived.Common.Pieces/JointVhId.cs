using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct JointVhId
    {
        public readonly VhId NodeVhId;
        public readonly byte Index;

        public JointVhId(VhId nodeVhId, byte index)
        {
            NodeVhId = nodeVhId;
            Index = index;
        }
    }
}
