using System;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Server.Helpers
{
    /// <summary>
    /// static helper class used to craf commonly used
    /// outbound messages for the server
    /// </summary>
    public static class ServerMessageHelper
    {
        public static NetOutgoingMessage BuildCreateNetworkEntityMessage(INetworkEntity networkEntity, IGroup group)
        {
            var om = group.CreateMessage("create");
            om.Write(networkEntity.Info.Handle);
            networkEntity.FullWrite(om);

            return om;
        }

        public static NetOutgoingMessage BuildUpdateNetworkEntityMessage(INetworkEntity networkEntity, IGroup group)
        {
            var om = group.CreateMessage("update");
            networkEntity.Write(om);

            return om;
        }

        public static NetOutgoingMessage BuildDestroyNetworkEntityMessage(INetworkEntity networkEntity, IGroup group)
        {
            var om = group.CreateMessage("destroy");
            om.Write(networkEntity.Id);

            return om;
        }
    }
}
