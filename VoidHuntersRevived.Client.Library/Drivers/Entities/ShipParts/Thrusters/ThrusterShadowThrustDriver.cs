using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities;
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
        private TrailManager _trail;
        #endregion

        #region Constructor
        public ThrusterShadowThrustDriver(TrailManager trail, ServerShadow server, Thruster driven) : base(driven)
        {
            _server = server;
            _trail = trail;
        }
        #endregion

        #region Lifecycle Methods
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Add a trail segment based on the thrusters current position 
            if(this.driven.Active)
                _trail.AddSegment(this.driven);

            // Apply thrust to the current thruster's root's shadow...
            this.driven.ApplyThrust(_server[this.driven.Root]);
        }
        #endregion
    }
}
