using Guppy.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Network
{
    internal sealed class ShipPlayerChangedMessage : NetworkEntityMessage<ShipPlayerChangedMessage>
    {
        public UInt16? PlayerNetworkId { get; set; }
    }
}
