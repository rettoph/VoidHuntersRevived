using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Joint
    {
        public readonly byte Index;
        public readonly FixLocation Location;

        public Joint(byte index, FixLocation location)
        {
            this.Index = index;
            this.Location = location;
        }
    }
}
