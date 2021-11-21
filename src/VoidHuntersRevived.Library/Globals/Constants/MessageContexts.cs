using Guppy.Network.Contexts;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class MessageContexts
    {
        public static class WorldObject
        {
            public readonly static NetOutgoingMessageContext WorldInfoPingMessageContext = new NetOutgoingMessageContext()
            {
                Method = NetDeliveryMethod.UnreliableSequenced,
                SequenceChannel = 1
            };
        }
    }
}
