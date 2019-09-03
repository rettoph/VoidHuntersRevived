using GalacticFighters.Library.Entities.ShipParts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Utilities.ConnectionNodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(Int32 id, ShipPart parent, float rotation, Vector2 position) : base(id, parent, rotation, position)
        {
        }
    }
}
