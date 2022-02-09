using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class Intervals
    {
        public const Double MinimumShipTargetPingInterval = 50;
        public const Double MaximumShipTargetPingInterval = 250;

        public const Double MinimumWorldInfoPingInterval = 50;
        public const Double MaximumWorldInfoPingInterval = 250;

        public static readonly Double ShipTargetPingBroadcastInterval = 100;

        /// <summary>
        /// The minimum amount of time before <see cref="AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent"/>
        /// will bother checking for a change.
        /// </summary>
        public static readonly Double AetherBodyWorldObjectCleanIntervalMinimum = 150;

        /// <summary>
        /// The maximum amount of time before <see cref="AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent"/>
        /// will broadcast the defined entity reguardless of dirty state.
        /// </summary>
        public static readonly Double AetherBodyWorldObjectCleanIntervalMaximum = 250;
    }
}
