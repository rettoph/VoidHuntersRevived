using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public abstract class ConnectionNode : DebuggableEntity
    {
        protected readonly ShipPart parent;

        public readonly Single Rotation;
        public readonly Vector2 Position;

        #region Constructors
        public ConnectionNode(ShipPart parent, Single rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            this.parent = parent;

            this.Rotation = rotation;
            this.Position = position;
        }
        #endregion
    }
}
