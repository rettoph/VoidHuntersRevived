using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    internal sealed class ShipTryToggleEnergyShieldsActionDriver : ShipContinuousSpellPartActionDriver<ShieldGenerator>
    {
        #region Protected Properties
        protected override uint TryActionId => VHR.Actions.Ship.TryToggleEnergyShields;

        protected override uint OnActionId => VHR.Actions.Ship.OnToggleEnergyShields;

        protected override bool DefaultActiveState => true;
        #endregion
    }
}
