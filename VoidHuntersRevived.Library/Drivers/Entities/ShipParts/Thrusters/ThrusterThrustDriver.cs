using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Main class in charge of managing a single thrusters
    /// Active state.
    /// 
    /// Note, this driver has a high priority and should run before
    /// and thrust utilizing drivers
    /// </summary>
    [IsDriver(typeof(Thruster), 90)]
    internal sealed class ThrusterThrustDriver : Driver<Thruster>
    {
        #region Constructor
        public ThrusterThrustDriver(Thruster driven) : base(driven)
        {
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.driven.Active = false;

            if (this.driven.Health > 0 && this.driven.Root.Ship != default(Ship))
            { // If the thruster is attached to a ship...
                if ((this.driven.Root.Ship.ActiveDirections & this.driven.Directions) != 0)
                { // If the thruster should be thrusting...
                    this.driven.Active = true;
                }
            }
        }
        #endregion
    }
}
