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
        private TrailManager _trails;
        private Single _trailStrength;
        private TrailManager.Trail _trail;
        #endregion

        #region Constructor
        public ThrusterShadowThrustDriver(TrailManager trail, ServerShadow server, Thruster driven) : base(driven)
        {
            _server = server;
            _trails = trail;
        }
        #endregion

        #region Lifecycle Methods
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Add a trail segment based on the thrusters current position 
            if (this.driven.Active)
            {
                
                _trailStrength = MathHelper.Lerp(_trailStrength, 0.5f, 0.01f);
            }
            else
            {
                _trailStrength = MathHelper.Lerp(_trailStrength, 0, 0.01f);
            }
                

            if(_trailStrength >= 0.01f)
            {
                if (_trail == null)
                    _trail = _trails.CreateTrail(this.driven);

                _trail.AddSegment(this.driven, _trailStrength, gameTime);
            }
            else
            {
                _trail = null;
            }
                

            // Apply thrust to the current thruster's root's shadow...
            this.driven.ApplyThrust(_server[this.driven.Root]);
        }
        #endregion
    }
}
