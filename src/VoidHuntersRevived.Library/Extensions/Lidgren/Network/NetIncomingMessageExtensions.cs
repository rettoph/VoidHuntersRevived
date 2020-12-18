using Guppy.Lists;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Extensions.Lidgren.Network
{
    public static class NetIncomingMessageExtensions
    {
        /// <summary>
        /// Deserialize <see cref="ConnectionNode"/> data that was
        /// serialized via the <see cref="NetOutgoingMessageExtensions.Write(NetOutgoingMessage, ConnectionNode)"/>
        /// extension method.
        /// </summary>
        /// <param name="om"></param>
        /// <param name="node"></param>
        public static ConnectionNode ReadConnectionNode(this NetIncomingMessage om, EntityList entities)
        {
            if (om.ReadBoolean())
                return entities.GetById<ShipPart>(om.ReadGuid()).FemaleConnectionNodes[om.ReadInt32()];

            return default;
        }
    }
}
