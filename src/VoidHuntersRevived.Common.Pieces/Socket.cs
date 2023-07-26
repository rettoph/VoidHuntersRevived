using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Socket
    {
        public readonly SocketId Id;
        public readonly Location Location;

        public Socket(EntityId nodeId, byte index, Location location)
        {
            this.Id = new SocketId(nodeId, index);
            this.Location = location;
        }
    }
}
