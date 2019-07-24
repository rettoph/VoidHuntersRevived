using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Extensions.Lidgren
{
    public static class NetOutgoingMessageExtensions
    {
        public static void Write(this NetOutgoingMessage om, FemaleConnectionNode node)
        {
            om.Write(node.Id);
            om.Write(node.Parent);
        }

        public static void Write(this NetOutgoingMessage om, MaleConnectionNode node)
        {
            om.Write(node.Parent);
        }
    }
}
