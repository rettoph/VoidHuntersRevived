using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Joint
    {
        public readonly EntityId NodeId;
        public readonly byte Index;
        public readonly Location Location;

        public Joint(EntityId nodeId, byte index, Location location)
        {
            this.NodeId = nodeId;
            this.Index = index;
            this.Location = location;
        }
    }
}
