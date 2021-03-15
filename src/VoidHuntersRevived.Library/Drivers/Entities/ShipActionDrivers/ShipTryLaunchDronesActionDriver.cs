using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    internal sealed class ShipTryLaunchDronesActionDriver : ShipActionDriver<DroneBay>
    {
        #region Protected Properties
        protected override uint TryActionId => VHR.Actions.Ship.TryLaunchDrones;

        protected override uint OnActionId => VHR.Actions.Ship.OnLaunchDrones;
        #endregion

        #region Helper Methods
        protected override bool TryAction(IEnumerable<DroneBay> parts, GameTime gameTime, params Object[] args)
        {
            foreach (DroneBay fighterBay in parts)
                    fighterBay.TryCast(gameTime);

            return true;
        }
        #endregion
    }
}
