using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Structs;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Extensions.Lidgren
{
    public static class NetIncomingMessageExtensions
    {
        public static TractorBeamAction ReadTractorBeamAction(this NetIncomingMessage im, ShipPartService shipParts, ShipPartSerializationFlags flags)
        {
            TractorBeamActionType type = im.ReadEnum<TractorBeamActionType>();
            ShipPart targetShipPart = default;
            ConnectionNode targetNode = default;

            if(im.ReadExists())
            {
                targetShipPart = shipParts.TryReadShipPart(im, flags);
            }

            if(im.ReadExists())
            {
                targetNode = shipParts.TryReadShipPart(im, flags).ConnectionNodes[im.ReadInt32()];
            }

            return new TractorBeamAction(
                type: type,
                targetShipPart: targetShipPart,
                targetNode: targetNode);
        }
    }
}
