using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    /// <summary>
    /// A tractor beam connection represents a rigid joint
    /// connecting a player (bridge) to another free-floating
    /// ship-part.
    /// 
    /// The joint is used by players to pick up and move around 
    /// ship-parts to attatch or detatch from their ship.
    /// </summary>
    public class TractorBeamConnection : Entity
    {
        public readonly Player Player;
        public readonly ShipPart Target;

        public Vector2 Offset { get; private set; }

        public TractorBeamConnection(Player player, ShipPart target, Vector2 offset, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            this.Player = player;
            this.Target = target;
        }
    }
}
