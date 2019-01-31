using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        public MaleConnectionNode(Vector3 connectionData, ShipPart owner, EntityInfo info, IServiceProvider provider, IGame game, SpriteBatch spriteBatch = null)
            : base("texture:connection_node:male", connectionData, owner, info, provider, game, spriteBatch)
        {
        }
    }
}
