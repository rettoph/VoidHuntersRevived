using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Extensions.Entities.ShipParts
{
    public static class ShipPartExtensions
    {
        #region Network Methods
        public static void WriteHealth(this ShipPart shipPart, NetOutgoingMessage om)
        {
            om.Write(shipPart.Health);
        }

        public static void ReadHealth(this ShipPart shipPart, NetIncomingMessage om)
        {
            if(shipPart == default(ShipPart))
            { // If the target does not exist we must still properly move the byte...
                om.ReadByte();
            }
            else
            { // Update the ShipParts health...
                shipPart.Health = om.ReadByte();
            }
        }
        #endregion
    }
}
