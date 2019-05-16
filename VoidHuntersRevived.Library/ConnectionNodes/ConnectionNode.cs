using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.ConnectionNodes
{
    public class ConnectionNode
    {
        protected Single rotation;
        protected Vector2 localPosition;
        protected ShipPart shipPart;

        public ConnectionNode(Vector2 localPosition, Single rotation, ShipPart shipPart)
        {
            this.shipPart = shipPart;
        }
    }
}
