using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(Vector3 connectionData, ShipPart owner, EntityInfo info, IServiceProvider provider, IGame game)
            : base("texture:connection_node:female", connectionData, owner, info, provider, game)
        {
        }
    }
}
