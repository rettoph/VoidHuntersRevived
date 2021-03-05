using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    public class PowerCell : RigidShipPart
    {
        #region Public Properties
        public new PowerCellContext Context { get; private set; }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as PowerCellContext;
        }
        #endregion
    }
}
