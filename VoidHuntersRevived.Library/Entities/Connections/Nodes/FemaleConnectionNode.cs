using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;

namespace VoidHuntersRevived.Library.Entities.Connections.Nodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(Vector3 connectionData, Hull owner, EntityInfo info, IServiceProvider provider, IGame game, SpriteBatch spriteBatch = null)
            : base("texture:connection_node:female", connectionData, owner, info, provider, game, spriteBatch)
        {
        }
    }
}
