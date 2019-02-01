using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Server.Helpers
{
    public static class ServerMessageHelper
    {
        public static NetOutgoingMessage BuildCreateMessage(INetworkEntity entity, IGroup group)
        {
            var om = group.CreateMessage("create");
            om.Write(entity.Info.Handle);
            entity.Write(om);

            return om;
        }

        public static NetOutgoingMessage BuildCreateNodeConnectionMessage(NodeConnection connection, IGroup group)
        {
            var femaleIndex = Array.IndexOf(
                connection.FemaleNode.Owner.FemaleConnectionNodes,
                connection.FemaleNode);

            // Create a new message containing the connection data
            var om = group.CreateMessage("create:node-connection");
            om.Write(connection.MaleNode.Owner.Id);
            om.Write(connection.FemaleNode.Owner.Id);
            om.Write(femaleIndex);

            return om;
        }
    }
}
