using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum ShipPartAmmunitionCollisionResult
    {
        /// <summary>
        /// Ignore the collision as a whole and continue onward.
        /// </summary>
        None,

        /// <summary>
        /// The collision should damage the current target.
        /// </summary>
        Damage = 1,

        /// <summary>
        /// The collision should stop looking for collisions.
        /// </summary>
        Stop = 2,

        /// <summary>
        /// The collision should damage the current target &
        /// stop looking for collisions.
        /// </summary>
        DamageAndStop = ShipPartAmmunitionCollisionResult.Damage | ShipPartAmmunitionCollisionResult.Stop
    }
}
