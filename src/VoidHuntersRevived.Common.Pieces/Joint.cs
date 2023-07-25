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
        public readonly JointId Id;
        public readonly Location Location;

        public Joint(EntityId nodeId, byte index, Location location)
        {
            this.Id = new JointId(nodeId, index);
            this.Location = location;
        }
    }
}
