using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Extensions.Lidgren.Network
{
    public static class NetOutgoingMessageExtensions
    {
        /// <summary>
        /// Serialize the recieved <paramref name="node"/> in such a way
        /// that a recieving <see cref="NetIncomingMessage"/> instance 
        /// can find the defined instance.
        /// </summary>
        /// <param name="om"></param>
        /// <param name="node"></param>
        public static void Write(this NetOutgoingMessage om, ConnectionNode node)
        {
            if(om.WriteExists(node))
            {
                om.Write(node.Parent.Id);
                om.Write(node.Id);
            }
        }
    }
}
