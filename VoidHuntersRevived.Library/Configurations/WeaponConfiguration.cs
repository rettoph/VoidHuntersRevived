using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    /// <summary>
    /// The main configuration class for weapon
    /// instances. This defines weapon specific data
    /// such as swivel range & fire rate.
    /// </summary>
    public class WeaponConfiguration : ShipPartConfiguration
    {
        /// <summary>
        /// The weapon's swivel range
        /// </summary>
        public Single SwivelRange { get; private set; } = MathHelper.PiOver2;
        /// <summary>
        /// The weapons fire rate in milliseconds
        /// </summary>
        public Double FireRate { get; private set; } = 150;

        /// <summary>
        /// Update the weapons swivel range.
        /// </summary>
        /// <param name="swivelRange"></param>
        public void SetSwivelRange(Single swivelRange)
        {
            this.SwivelRange = swivelRange;
        }


        public void SetFireRate(Double fireRate)
        {
            this.FireRate = fireRate;
        }
    }
}
