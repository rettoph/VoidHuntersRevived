using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Dtos.Utilities
{
    public class ConnectionNodeDto
    {
        public Byte Index { get; set; }
        public ConnectionNodeState State { get; set; }
        public Byte? TargetNodeIndex { get; set; }
        public ShipPartPacket TargetNodeOwner { get; set; }
    }
}
