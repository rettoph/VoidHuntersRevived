using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Armors
{
    public class Armor : RigidShipPart
    {
        #region Public Properties
        public new ArmorContext Context { get; private set; }
        #endregion

        #region Helper Methods
        public override void TryApplyDamage(Single damage)
        { // There is no damage to apply hehe
            // base.TryDamage(damage);
        }

        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as ArmorContext;
        }
        #endregion
    }
}
