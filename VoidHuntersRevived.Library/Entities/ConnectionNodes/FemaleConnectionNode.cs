using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(ShipPart parent, float rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(parent, rotation, position, configuration, scene, provider, logger)
        {
        }
    }
}
