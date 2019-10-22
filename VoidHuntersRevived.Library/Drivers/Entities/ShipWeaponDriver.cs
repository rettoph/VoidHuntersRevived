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
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    internal sealed class ShipWeaponDriver : ShipShipPartDriver<Weapon>
    {
        #region Constructor
        public ShipWeaponDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.parts.ForEach(w =>
            {
                if (w.Health > 10)
                {
                    w.UpdateBarrelTarget(this.driven.Target);
                    if (this.driven.Firing)
                        w.TryFire();
                }
            });
        }
        #endregion
    }
}
