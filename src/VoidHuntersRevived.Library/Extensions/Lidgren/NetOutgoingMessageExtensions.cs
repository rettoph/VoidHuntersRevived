using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Structs;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Extensions.Lidgren
{
    public static class NetOutgoingMessageExtensions
    {
        public static void Write(this NetOutgoingMessage om, TractorBeamAction action, ShipPartService shipParts, ShipPartSerializationFlags flags)
        {
            om.Write<TractorBeamActionType>(action.Type);

            if (om.WriteExists(action.TargetPart))
            {
                shipParts.WriteShipPart(action.TargetPart, om, flags);
            }

            if (om.WriteExists(action.TargetNode))
            {
                shipParts.WriteShipPart(action.TargetNode.Owner, om, flags);
                om.Write(action.TargetNode.Index);
            }
        }
    }
}
