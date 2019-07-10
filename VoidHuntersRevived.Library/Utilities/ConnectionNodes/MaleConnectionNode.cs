using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Guppy.Extensions.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Utilities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position, ContentLoader content, SpriteBatch spriteBatch = null) : base(-1, parent, rotation, position, spriteBatch)
        {
            this.texture = content.Get<Texture2D>("texture:connection-node:male");
        }
    }
}
