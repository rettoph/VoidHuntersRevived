using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected override bool TryAction(IEnumerable<DroneBay> parts, GameTime gameTime, ref Byte data)
        {
            Boolean response = false;

            foreach (DroneBay fighterBay in parts)
                if (fighterBay.TryCast(gameTime) != default)
                    response = true;

            return response;
        }
        #endregion
    }
}
