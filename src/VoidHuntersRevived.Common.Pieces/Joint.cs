using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Joint
    {
        public readonly byte Index;
        public readonly Location Location;

        public Joint(byte index, Location location)
        {
            this.Index = index;
            this.Location = location;
        }
    }
}
