using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    public class FighterBay : RigidShipPart
    {
        #region Events
        public event OnEventDelegate<FighterBay> OnLaunch;
        #endregion

        #region API Methods
        public void TryLaunch()
        {
            this.OnLaunch?.Invoke(this);
        }
        #endregion
    }
}
