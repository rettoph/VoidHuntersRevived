using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Applies thrust to the thruster's server shadow
    /// body when needed.
    /// </summary>
    [IsDriver(typeof(Thruster))]
    internal sealed class ThrusterShadowThrustDriver : Driver<Thruster>
    {
        #region Private Fields
        private ServerShadow _server;
        private TrailManager _trails;
        #endregion

        #region Constructor
        public ThrusterShadowThrustDriver(TrailManager trails, ServerShadow server, Thruster driven) : base(driven)
        {
            _server = server;
            _trails = trails;
        }
        #endregion

        #region Lifecycle Methods
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Attempt to add a segment to the trai manager
            _trails.TryAddTrail(this.driven);

            // Apply thrust to the current thruster's root's shadow...
            this.driven.ApplyThrust(_server[this.driven.Root]);
        }
        #endregion
    }
}
