using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        public MaleConnectionNode(
            ShipPart owner,
            Vector3 connectionData,
            EntityInfo info,
            IGame game,
            SpriteBatch spriteBatch = null) : base("texture:connection_node:male", owner, connectionData, info, game, spriteBatch)
        {
        }
    }
}
