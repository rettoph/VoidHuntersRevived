using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    internal sealed class ShipThrusterDriver : ShipShipPartDriver<Thruster>
    {
        #region Constructor
        public ShipThrusterDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.parts.ForEach(t =>
            {
                if (t.Health > 10)
                    t.TryThrust(this.driven.ActiveDirections);
            });
        }
        #endregion
    }
}
