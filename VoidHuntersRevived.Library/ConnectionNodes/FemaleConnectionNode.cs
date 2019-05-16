using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.ConnectionNodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(Vector2 localPosition, Single rotation, ShipPart shipPart) : base(localPosition, rotation, shipPart)
        {
        }
    }
}
