using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Server.Helpers
{
    public static class ServerMessageHelper
    {
        public static NetOutgoingMessage BuildCreateMessage(INetworkEntity entity, ServerGroup group)
        {
            var om = group.CreateMessage("create");
            om.Write(entity.Info.Handle);
            entity.Write(om);

            return om;
        }
    }
}
