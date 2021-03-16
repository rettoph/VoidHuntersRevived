using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Extensions.Entities
{
    public static class ShipExtensions
    {
        #region
        /// <summary>
        /// Helper method to automatically launch fighters. 
        /// The handling is done is a custom helper class.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="gameTime"></param>
        public static Boolean TryLaunchFighters(this Ship ship, GameTime gameTime)
            => ship.Actions.TryInvoke(VHR.Actions.Ship.TryLaunchDrones, gameTime);

        /// <summary>
        /// Helper method to automatically toggle shield states. 
        /// The handling is done is a custom helper class.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="gameTime"></param>
        /// <param name="target"></param>
        public static Boolean TryToggleEnergyShields(this Ship ship, GameTime gameTime, ContinuousSpellPartTargetState target = ContinuousSpellPartTargetState.Toggle)
            => ship.Actions.TryInvoke(VHR.Actions.Ship.TryToggleEnergyShields, gameTime, (Byte)target);
        #endregion
    }
}
