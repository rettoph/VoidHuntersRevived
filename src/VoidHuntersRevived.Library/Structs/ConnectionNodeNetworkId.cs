using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Structs
{
    public struct ConnectionNodeNetworkId
    {
        public readonly UInt16 OwnerNetworkId;
        public readonly Byte Index;

        public ConnectionNodeNetworkId(ushort ownerNetworkId, byte index)
        {
            this.OwnerNetworkId = ownerNetworkId;
            this.Index = index;
        }
    }
}
